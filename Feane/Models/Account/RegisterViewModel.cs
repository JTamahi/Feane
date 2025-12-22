using System.ComponentModel.DataAnnotations;

namespace Feane.Models.Account;

public sealed class RegisterViewModel
{
    [Required(ErrorMessage = "User name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "User name must be 2..50 chars")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 chars")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = "";
}
