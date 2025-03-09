using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IAppService
{
    public interface IProposalAppService
    {
        Task<List<ProposalDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<ProposalDto>> GetProposalsByOrderIdAsync(int orderId, CancellationToken cancellationToken);
        Task<ProposalDto> GetProposalByIdAsync(int proposalId, CancellationToken cancellationToken);
        Task UpdateProposalAsync(Proposal proposal, CancellationToken cancellationToken);

        Task<List<ProposalDto>> GetProposalsByExpertIdAsync(int expertId, CancellationToken cancellationToken);
        Task<bool> CreateProposalAsync(CreateProposalDto dto, CancellationToken cancellationToken);

        Task<List<ProposalDto>> GetProposalsByRequestIdAsync(int requestId, CancellationToken cancellationToken);
    }

}