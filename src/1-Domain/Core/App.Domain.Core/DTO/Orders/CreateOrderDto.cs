using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Orders
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public int ExpertId { get; set; }
        public int RequestId { get; set; }
        public int ProposalId { get; set; }
        public decimal FinalPrice { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
