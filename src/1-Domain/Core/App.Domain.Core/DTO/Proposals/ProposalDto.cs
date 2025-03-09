using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Proposals
{
    public class ProposalDto
    {
        public int Id { get; set; }
        public int ExpertId { get; set; }
        public string ExpertName { get; set; }
        public int RequestId { get; set; }
        public string RequestDescription { get; set; }
        public int? OrderId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public decimal Price { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Description { get; set; }
        public ProposalStatus Status { get; set; }
        public DateTime ResponseTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEnabled { get; set; }
        public string SubHomeServiceName { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }

}