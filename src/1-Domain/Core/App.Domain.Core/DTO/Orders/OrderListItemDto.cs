using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Orders
{
    public class OrderListItemDto
    {
        public int Id { get; set; }
        public decimal FinalPrice { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ExpertName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
