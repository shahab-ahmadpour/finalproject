using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IService
{
    public interface IProposalService
    {
        Task<List<ProposalDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<ProposalDto>> GetProposalsByOrderIdAsync(int orderId, CancellationToken cancellationToken);
        Task<List<ProposalDto>> GetProposalsByExpertIdAsync(int expertId, CancellationToken cancellationToken); 
    }

}
