using App.Domain.Core.Enums;
using App.Domain.Core.Skills.Entities;
using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Entities
{
    public class Proposal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ExpertId { get; set; }

        [Required]
        public Expert Expert { get; set; } = null!;

        [Required]
        public int RequestId { get; set; }
        [Required]
        public Request Request { get; set; } = null!;

        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public int SkillId { get; set; }

        [Required]
        public Skill Skill { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime ExecutionDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public ProposalStatus Status { get; set; }

        [Required]
        public DateTime ResponseTime { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsEnabled { get; set; } = true;
    }
}
