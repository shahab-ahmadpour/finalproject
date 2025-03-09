using App.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace App.Endpoints.MVC.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأیید رمز عبور الزامی است.")]
        [Compare("Password", ErrorMessage = "رمز عبور و تأیید آن یکسان نیستند.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "نقش الزامی است.")]
        public string Role { get; set; }
    }


}
