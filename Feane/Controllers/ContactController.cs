//namespace Feane.Controllers
//{
//    public class ContactController
//    {
//    }
//}
using Feane.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feane.Controllers
{
    public class ContactController : Controller
    {
        [Authorize]

        public IActionResult Index(ContactFrom form)
        {
            ContactFormValidator validator = new ContactFormValidator();
            var result = validator.Validate(form);
            if (!result.IsValid)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);

                }
            }
            //if (string.IsNullOrWhiteSpace(form.name))
            // ModelState.AddModelError("name", "Укажите свое имя");

            return View(form);
        }
        [HttpPost]
        public IActionResult SaveData(ContactFrom form)
        {
            //1 string name, string email, string message
            //2
            //if (string.IsNullOrWhiteSpace(form.name))
            //    ModelState.AddModelError("name", "Укажите свое имя");
            if (ModelState.IsValid)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", new { ContactFrom = form });
            }

            //var _name = Request.Form["name"];
            //var _email = Request.Form["email"];
            //var _message = Request.Form["message"];
            //3
            ViewBag.Result = "Ваше сообщение отправлено!";
            TempData["Result"] = "Ваше сообщение отправлено!";
            return RedirectToAction("Index");
            //return View();
        }
    }
}