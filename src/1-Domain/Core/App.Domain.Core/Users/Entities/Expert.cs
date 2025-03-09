using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Skills.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Entities
{
    public class Expert
    {
        [Key]
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;


        [MaxLength(15), Phone]
        public string PhoneNumber { get; set; } = null!;
        [MaxLength(50)]
        public string Address { get; set; } = null!;
        [MaxLength(20)]
        public string City { get; set; } = null!;
        [MaxLength(20)]
        public string State { get; set; } = null!;


        public ICollection<ExpertSkill> ExpertSkills { get; set; } = new List<ExpertSkill>();
        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
}
