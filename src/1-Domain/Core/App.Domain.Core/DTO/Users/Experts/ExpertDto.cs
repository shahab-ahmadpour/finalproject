using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.Experts
{
    public class ExpertDto : AppUserDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public int AppUserId { get; set; }
    }

}
