using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.ExpertSkills
{
    public class ExpertSkillDetailDto
    {
        public int Id { get; set; }
        public int ExpertId { get; set; }
        public string ExpertName { get; set; } = null!;
        public int SkillId { get; set; }
        public string SkillName { get; set; } = null!;
    }
}
