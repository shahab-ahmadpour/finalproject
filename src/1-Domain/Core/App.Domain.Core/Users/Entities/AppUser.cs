using App.Domain.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Entities
{
    public class AppUser : IdentityUser<int>
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = "Default";

        [Required, MaxLength(50)]
        public string LastName { get; set; } = "Default";

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal AccountBalance { get; set; } = 0;

        public string ProfilePicture { get; set; } = "default.png";
        public bool IsEnabled { get; set; } = false;
        public bool IsConfirmed { get; set; } = false;

        [Required, EmailAddress]
        public override string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;

        public Customer? Customer { get; set; }
        public Expert? Expert { get; set; }
        public Admin? Admin { get; set; }
    }

}