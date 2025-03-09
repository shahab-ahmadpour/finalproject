using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Proposals
{
    public class ProposalListItemDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string ExpertName { get; set; } = null!;
        public DateTime ResponseTime { get; set; }
    }
}
