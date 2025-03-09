using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Skills.Entities
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, ForeignKey("SubHomeService")]
        public int SubHomeServiceId { get; set; }
        public SubHomeService SubHomeService { get; set; } = null!;

        public List<ExpertSkill> ExpertSkills { get; set; } = new();

    }
}
