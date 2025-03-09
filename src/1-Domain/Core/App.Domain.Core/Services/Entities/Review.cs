using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [Required]
        public int ExpertId { get; set; }
        public Expert Expert { get; set; } = null!;

        [Required]
        public int OrderId { get; set; }

        [Required]
        public Order Order { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public bool IsApproved { get; set; } = false;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
