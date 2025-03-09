using App.Domain.Core._ِDTO.ExpertSkills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Users.Experts
{
    public class ExpertDetailDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal AccountBalance { get; set; }
        public string ProfilePicture { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public ICollection<ExpertSkillDetailDto> ExpertSkills { get; set; } = new List<ExpertSkillDetailDto>();
    }

}
