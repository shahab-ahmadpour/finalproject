using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Orders
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public decimal FinalPrice { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public int ExpertId { get; set; }
    }
}
