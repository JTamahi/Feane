using Feane.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Feane.Pages.Book;

public class BookModelPage : PageModel
{
    private readonly ILogger<BookModelPage> _logger;

    public BookModelPage(ILogger<BookModelPage> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public BookingModel Booking { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            // лог в Output, возвращаем страницу с ошибками
            foreach (var kv in ModelState)
                foreach (var err in kv.Value.Errors)
                    _logger.LogWarning("ModelState error {Key}: {Error}", kv.Key, err.ErrorMessage);

            return Page();
        }

        TempData["Success"] = "Бронирование успешно принято";
        return RedirectToPage(); // Redirect-After-Post
    }
}