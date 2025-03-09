using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Domain.Core.Users.Interfaces.IService;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.AppServices.ProposalAppServices
{
    public class ProposalAppService : IProposalAppService
    {
        private readonly IProposalService _proposalService;
        private readonly ICustomerService _customerService;
        private readonly IProposalRepository _proposalRepository;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public ProposalAppService(
            IProposalService proposalService,
            ICustomerService customerService,
            IProposalRepository proposalRepository,
            ILogger logger,
            IMemoryCache memoryCache)
        {
            _proposalService = proposalService;
            _customerService = customerService;
            _proposalRepository = proposalRepository;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<List<ProposalDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all proposals.");
            return await _proposalService.GetAllAsync(cancellationToken);
        }

        public async Task<List<ProposalDto>> GetProposalsByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Getting proposals for order ID: {OrderId}", orderId);
            string cacheKey = $"Proposals_Order_{orderId}";

            if (!_memoryCache.TryGetValue(cacheKey, out List<ProposalDto> cachedProposals))
            {
                _logger.Information("Proposals not found in cache for OrderId: {OrderId}, fetching from database", orderId);
                cachedProposals = await _proposalService.GetProposalsByOrderIdAsync(orderId, cancellationToken);
                if (cachedProposals != null && cachedProposals.Any())
                {
                    _logger.Information("Caching {ProposalCount} proposals for OrderId: {OrderId}", cachedProposals.Count, orderId);
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    };
                    _memoryCache.Set(cacheKey, cachedProposals, cacheOptions);
                }
            }
            else
            {
                _logger.Information("Proposals retrieved from cache for OrderId: {OrderId}, Count: {ProposalCount}", orderId, cachedProposals.Count);
            }
            return cachedProposals;
        }

        public async Task<ProposalDto> GetProposalByIdAsync(int proposalId, CancellationToken cancellationToken)
        {
            _logger.Information("ProposalAppService: Fetching proposal by ID: {Id}", proposalId);
            return await _customerService.GetProposalByIdAsync(proposalId, cancellationToken);
        }

        public async Task UpdateProposalAsync(Proposal proposal, CancellationToken cancellationToken)
        {
            _logger.Information("ProposalAppService: Updating proposal with ID: {Id}", proposal.Id);
            await _customerService.UpdateProposalAsync(proposal, cancellationToken);
            _logger.Information("ProposalAppService: Successfully updated proposal with ID: {Id}", proposal.Id);
        }

        public async Task<List<ProposalDto>> GetProposalsByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Getting proposals for expert ID: {ExpertId}", expertId);

            string cacheKey = $"Proposals_Expert_{expertId}";
            if (!_memoryCache.TryGetValue(cacheKey, out List<ProposalDto> cachedProposals))
            {
                try
                {
                    var proposals = await _proposalRepository.GetProposalsByExpertIdAsync(expertId, cancellationToken);
                    if (proposals != null && proposals.Any())
                    {
                        _logger.Information("Caching {ProposalCount} proposals for ExpertId: {ExpertId}", proposals.Count, expertId);
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        };
                        _memoryCache.Set(cacheKey, proposals, cacheOptions);
                    }

                    return proposals;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to fetch proposals for ExpertId: {ExpertId}", expertId);
                    return new List<ProposalDto>();
                }
            }
            else
            {
                _logger.Information("Proposals retrieved from cache for ExpertId: {ExpertId}, Count: {ProposalCount}",
                    expertId, cachedProposals.Count);
                return cachedProposals;
            }
        }

        public async Task<bool> CreateProposalAsync(CreateProposalDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Creating new proposal for ExpertId: {ExpertId}, RequestId: {RequestId}",
                dto.ExpertId, dto.RequestId);

            try
            {
                var proposal = new Proposal
                {
                    ExpertId = dto.ExpertId,
                    RequestId = dto.RequestId,
                    SkillId = dto.SkillId,
                    Price = dto.Price,
                    ExecutionDate = dto.ExecutionDate,
                    Description = dto.Description,
                    Status = dto.Status,
                    ResponseTime = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    IsEnabled = true
                };

                await _proposalRepository.CreateAsync(proposal, cancellationToken);

                // Invalidate cache
                _memoryCache.Remove($"Proposals_Expert_{dto.ExpertId}");

                _logger.Information("Successfully created proposal for ExpertId: {ExpertId}, RequestId: {RequestId}",
                    dto.ExpertId, dto.RequestId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create proposal for ExpertId: {ExpertId}, RequestId: {RequestId}",
                    dto.ExpertId, dto.RequestId);
                return false;
            }
        }


        public async Task<List<ProposalDto>> GetProposalsByRequestIdAsync(int requestId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Getting proposals for RequestId: {RequestId}", requestId);

            string cacheKey = $"Proposals_Request_{requestId}";
            if (!_memoryCache.TryGetValue(cacheKey, out List<ProposalDto> cachedProposals))
            {
                try
                {
                    var proposals = await _proposalRepository.GetProposalsByRequestIdAsync(requestId, cancellationToken);
                    if (proposals != null && proposals.Any())
                    {
                        _logger.Information("Caching {ProposalCount} proposals for RequestId: {RequestId}", proposals.Count, requestId);
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        };
                        _memoryCache.Set(cacheKey, proposals, cacheOptions);
                    }

                    return proposals ?? new List<ProposalDto>();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to fetch proposals for RequestId: {RequestId}", requestId);
                    return new List<ProposalDto>();
                }
            }
            else
            {
                _logger.Information("Proposals retrieved from cache for RequestId: {RequestId}, Count: {ProposalCount}",
                    requestId, cachedProposals.Count);
                return cachedProposals;
            }
        }
    }
}