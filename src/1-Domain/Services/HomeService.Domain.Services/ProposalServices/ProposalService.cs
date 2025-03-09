using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.ProposalServices
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly ILogger _logger;

        public ProposalService(IProposalRepository proposalRepository, ILogger logger)
        {
            _proposalRepository = proposalRepository;
            _logger = logger;
            
        }

        public async Task<List<ProposalDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all proposals.");
            return await _proposalRepository.GetAllAsync(cancellationToken);
        }

        public async Task<List<ProposalDto>> GetProposalsByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Getting proposals for order ID: {OrderId}", orderId);
            return await _proposalRepository.GetProposalsByOrderIdAsync(orderId, cancellationToken);
        }
        public async Task<List<ProposalDto>> GetProposalsByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching proposals for ExpertId: {ExpertId}", expertId);
            try
            {
                var proposals = await _proposalRepository.GetProposalsByExpertIdAsync(expertId, cancellationToken);
                if (proposals == null || !proposals.Any())
                {
                    _logger.Warning("No proposals found for ExpertId: {ExpertId}", expertId);
                    return new List<ProposalDto>();
                }
                return proposals;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching proposals for ExpertId: {ExpertId}", expertId);
                throw;
            }
        }
    }
}
