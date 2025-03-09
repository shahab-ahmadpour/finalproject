using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.Experts
{
    public class EditExpertDto
    {
        public int AppUserId { get; set; }

        [Required(ErrorMessage = "نام اجباری است")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "نام خانوادگی اجباری است")]
        public string LastName { get; set; }

        public string ProfilePicture { get; set; }

        [Required(ErrorMessage = "شماره تماس اجباری است")]
        [Phone(ErrorMessage = "فرمت شماره تماس نامعتبر است")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "آدرس اجباری است")]
        public string Address { get; set; }

        [Required(ErrorMessage = "شهر اجباری است")]
        public string City { get; set; }

        [Required(ErrorMessage = "استان اجباری است")]
        public string State { get; set; }

        public IFormFile ProfilePictureFile { get; set; }
    }
}