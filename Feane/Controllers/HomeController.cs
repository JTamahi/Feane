using Feane.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Feane.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // логируем детально (в логи можно exception)
            _logger.LogError(feature?.Error, "Unhandled exception on path {Path}", feature?.Path);

            return View(new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            });
        }

        [HttpGet]
        [HttpGet]
        public IActionResult StatusCode(int code)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var originalPath = feature?.OriginalPath ?? HttpContext.Request.Path.Value ?? "";

            var accept = HttpContext.Request.Headers.Accept.ToString();

            var isApi =
                originalPath.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
                accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);

            if (isApi)
            {
                var details = new ProblemDetails
                {
                    Title = "Request error",
                    Status = code,
                    Detail = code == 404 ? "Resource not found." : "Request cannot be processed.",
                    Instance = $"{Request.Method} {originalPath}"
                };
                details.Extensions["correlationId"] = HttpContext.TraceIdentifier;

                return StatusCode(code, details);
            }

            ViewBag.Code = code;
            return View("StatusCode");
        }



        [HttpGet]
        public IActionResult Throw()
        {
            throw new Exception("Test MVC exception");
        }
    }
}
