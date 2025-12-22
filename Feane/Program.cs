using System.Globalization;
using Feane.Middleware;
using Feane.Models;
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

// MVC
builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// =====================
// 3) Build app
// =====================
var app = builder.Build();

// =====================
// 4) Middleware pipeline
// =====================

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// (позже сюда добавим: CorrelationIdMiddleware, RequestLoggingMiddleware, ProblemDetails и т.д.)

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Serilog request logging (все входящие запросы)
app.UseSerilogRequestLogging(options =>
{
    // важно: добавляем свойства прямо в событие "Request finished"
    options.IncludeQueryInRequestPath = true;

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name ?? "anonymous");
        diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? "");
    };

    // чтобы в тексте "Request finished..." сразу было видно CorrelationId
    options.MessageTemplate =
        "Request finished {RequestMethod} {RequestPath} -> {StatusCode} in {Elapsed:0.0000} ms " +
        "(CorrelationId={CorrelationId}, UserName={UserName})";
});


// Localization (culture cookie)
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

app.UseAuthentication();
app.UseAuthorization();

// =====================
// 5) Endpoints
// =====================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
