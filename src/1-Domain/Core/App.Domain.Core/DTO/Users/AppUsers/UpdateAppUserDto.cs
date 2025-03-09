using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.AppUsers
{
    public class UpdateAppUserDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsConfirmed { get; set; }
        public UserRole Role { get; set; }
        public string? Password { get; set; }
        public bool IsEnabled { get; set; }
        public decimal? AccountBalance { get; set; } 
    }
}
