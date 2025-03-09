using App.Domain.Core.Enums;
using App.Domain.Core.Skills.Entities;
using App.Domain.Core.Users.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Entities
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public Customer Customer { get; set; } = null!;

        [Required]
        public int SubHomeServiceId { get; set; }

        [Required]
        public SubHomeService SubHomeService { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public RequestStatus Status { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public DateTime ExecutionDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? EnvironmentImagePath { get; set; }

        [Required]
        public bool IsEnabled { get; set; } = true;

        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
