using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Transactions
{
    public class UpdateTransactionDto
    {
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } 
        public bool? IsActive { get; set; } 
    }

}
