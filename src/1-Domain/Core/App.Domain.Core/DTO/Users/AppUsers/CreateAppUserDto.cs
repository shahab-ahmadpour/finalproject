using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.AppUsers
{
    public class CreateAppUserDto
    {
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        public string Password { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = "default.png";
        public decimal AccountBalance { get; set; } = 0;

        [Required(ErrorMessage = "نقش الزامی است.")]
        public UserRole Role { get; set; } = UserRole.Customer;

        public bool IsEnabled { get; set; } = false;
        public bool IsConfirmed { get; set; } = false;
    }

}
