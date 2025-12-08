using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Feane.Controllers
{
    public class CultureController : Controller
    {
        // GET /Culture/Set?culture=ru&returnUrl=/Book
        [HttpGet]
        public IActionResult Set(string culture, string returnUrl)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, cookieValue, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false
            });

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}