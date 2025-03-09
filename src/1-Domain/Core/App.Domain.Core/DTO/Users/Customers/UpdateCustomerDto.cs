﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.Customers
{
    public class UpdateCustomerDto
    {
        public int CustomerId { get; set; } 
        public int AppUserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
