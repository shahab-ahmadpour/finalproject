using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Transactions
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal AmountToPay { get; set; }
        public string ExpertName { get; set; }
    }
}
