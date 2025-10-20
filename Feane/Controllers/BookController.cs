using Microsoft.AspNetCore.Mvc;

namespace Feane.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Book()
        {
            return View();
        }
    }
}
