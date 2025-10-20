using Microsoft.AspNetCore.Mvc;

namespace Feane.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
