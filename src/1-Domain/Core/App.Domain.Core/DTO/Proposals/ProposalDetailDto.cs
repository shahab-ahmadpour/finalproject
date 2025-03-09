using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Proposals
{
    public class ProposalDetailDto
    {
        public int Id { get; set; }
        public int ExpertId { get; set; }
        public int RequestId { get; set; }
        public decimal Price { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
