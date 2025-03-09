using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Orders
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public bool IsActive { get; set; } = true;
        public RequestStatus Status { get; set; }
        public DateTime CompletionDate { get; set; }
        public string CustomerName { get; set; }
        public string ExpertName { get; set; }
        public string RequestDescription { get; set; }
    }
}
