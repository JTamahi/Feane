using System.Diagnostics;
using Feane.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;

namespace Feane.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext db;
        private readonly IViewLocalizer _localizer;

        public HomeController(ILogger<HomeController> logger, AppDbContext db, IViewLocalizer localizer)
        {
            _logger = logger;
            this.db = db;
            _localizer = localizer;
        }

        /*        public IActionResult Index()
                {
                    return View();
                }*/

        public async Task<IActionResult> Index()
        {
            var products = await db.Products.ToListAsync();
            return View(products);
        }

        /*        public IActionResult Index()
                {
                    List<Product> products = db.Products.ToList();

                    return View(products);
                }*/

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
