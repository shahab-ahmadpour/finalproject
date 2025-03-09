using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.Customers
{
    public class CustomerDto : AppUserDto
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

}
