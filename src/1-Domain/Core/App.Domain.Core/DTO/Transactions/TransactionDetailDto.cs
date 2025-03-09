using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Transactions
{
    public class TransactionDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime TransactionDate { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string ExpertName { get; set; } = string.Empty;
    }

}
