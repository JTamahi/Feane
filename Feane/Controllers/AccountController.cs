using Microsoft.AspNetCore.Mvc;

namespace Feane.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LogInOut()
        {
            return View();
        }
    }
}
