using App.Domain.Core.Enums;
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
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [ForeignKey("Expert")]
        public int ExpertId { get; set; }
        public Expert Expert { get; set; } = null!;

        [ForeignKey("Request")]
        public int RequestId { get; set; }
        public Request Request { get; set; } = null!;

        public int? ProposalId { get; set; }
        public Proposal Proposal { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalPrice { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public PaymentStatus PaymentStatus { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
