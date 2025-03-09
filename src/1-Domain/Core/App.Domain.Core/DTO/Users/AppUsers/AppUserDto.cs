using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.AppUsers
{
    public class AppUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ProfilePicture { get; set; } = "default.png";
        public decimal AccountBalance { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? CreatedAt { get; set; }
        public UserRole Role { get; set; }
    }

}
