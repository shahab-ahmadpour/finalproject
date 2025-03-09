using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Transactions.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required, ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [Required, ForeignKey("Expert")]
        public int ExpertId { get; set; }
        public Expert Expert { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public TransactionType TransactionType { get; set; }
   
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
