using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Proposals
{
    public class UpdateProposalDto
    {
        public ProposalStatus Status { get; set; }
    }
}
