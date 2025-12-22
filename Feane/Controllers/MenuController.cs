using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feane.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext db;

        public MenuController(AppDbContext context)
        {
            db = context;
        }
        /*        public IActionResult Menu()
                {
                    return View();
                }*/
        public async Task<IActionResult> Menu()
        {
            var products = await db.Products.ToListAsync();
            return View(products);
        }
    }
}
