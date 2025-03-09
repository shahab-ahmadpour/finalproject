using System.ComponentModel.DataAnnotations;

namespace App.Endpoints.MVC.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "وارد کردن ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "لطفاً یک ایمیل معتبر وارد کنید.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

