using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Skills.Entities
{
    public class ExpertSkill
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Expert")]
        public int ExpertId { get; set; }
        public Expert Expert { get; set; } = null!;

        [Required, ForeignKey("Skill")]
        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
