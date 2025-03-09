using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Transactions
{
    public class CreateTransactionDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int ExpertId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; } 
        public bool IsActive { get; set; } 
    }

}
