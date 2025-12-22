using System.Globalization;
using Feane.Middleware;
using Feane.Models;
using Feane.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// =====================
// 1) Logging (Serilog)
// =====================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
                "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} " +
                "(CorrelationId={CorrelationId}, UserName={UserName}, RequestPath={RequestPath}){NewLine}{Exception}")

    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// =====================
// 2) Services (DI)
// =====================

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// EF Core
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (AuthN/AuthZ)
builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

// MVC + Filters
builder.Services.AddScoped<RequestTimingResourceFilter>();
builder.Services.AddScoped<AuditActionFilter>();
builder.Services.AddScoped<ResponseHeadersResultFilter>();


builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<RequestTimingResourceFilter>();
    options.Filters.AddService<AuditActionFilter>();
    options.Filters.AddService<ResponseHeadersResultFilter>();

})
.AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization();



builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["correlationId"] = ctx.HttpContext.TraceIdentifier;
    };
});


// =====================
// 3) Build app
// =====================
var app = builder.Build();

// =====================
// 4) Middleware pipeline
// =====================

// 1) Сначала correlationId и твой request-лог
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// 2) Потом Serilog request logging (чтобы логировал и ошибки тоже)
app.UseSerilogRequestLogging(options =>
{
    options.IncludeQueryInRequestPath = true;

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name ?? "anonymous");
        diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? "");
    };

    options.MessageTemplate =
        "Request finished {RequestMethod} {RequestPath} -> {StatusCode} in {Elapsed:0.0000} ms " +
        "(CorrelationId={CorrelationId}, UserName={UserName})";
});

// 3) Потом глобальная обработка ошибок (HTML для сайта, JSON для /api)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var path = context.Request.Path.Value ?? "";

        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var pd = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "Unhandled server error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred.",
                Instance = $"{context.Request.Method} {context.Request.Path}"
            };

            pd.Extensions["correlationId"] = context.TraceIdentifier;

            await context.Response.WriteAsJsonAsync(pd);
            return;
        }

        context.Response.Redirect("/Home/Error");
    });
});

// ВАЖНО: чтобы в Development показывалась твоя Error-страница,
// ВРЕМЕННО закомментируй DeveloperExceptionPage
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 4) Дальше обычный pipeline
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ru")
};

var requestLocalizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(requestLocalizationOptions);

app.UseStaticFiles();

app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.UseAuthentication();
app.UseAuthorization();

// =====================
// 5) Endpoints
// =====================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
