using App.Domain.Core.Skills.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App.Domain.Core.Services.Entities
{
    public class SubHomeService
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0, int.MaxValue)]
        public int Views { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [MaxLength(255)]
        public string ImagePath { get; set; }

        [Required, ForeignKey("HomeService")]
        public int HomeServiceId { get; set; }
        public bool IsActive { get; set; } = true;

        public HomeService HomeService { get; set; } = null!;

        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
