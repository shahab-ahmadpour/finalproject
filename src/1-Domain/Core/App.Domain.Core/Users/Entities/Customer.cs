using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;

        [MaxLength(15), Phone]
        public string PhoneNumber { get; set; } = null!;
        [MaxLength(50)]
        public string Address { get; set; } = null!;
        [MaxLength(15)]
        public string City { get; set; } = null!;
        [MaxLength(15)]
        public string State { get; set; } = null!;

        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
