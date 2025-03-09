using App.Domain.Core.DTO.Requests;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using App.Domain.Core.Skills.Interfaces;
using App.Domain.Core.Skills.Interfaces.IService;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Domain.Core.Users.Interfaces.IService;
using HomeService.Domain.Services.ProposalServices;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.AppServices.RequestAppServices
{
    public class RequestAppService : IRequestAppService
    {
        private readonly IRequestService _requestService;
        private readonly IExpertService _expertService;
        private readonly ICustomerService _customerService;
        private readonly IProposalService _proposalService;
        private readonly ISkillService _skillService;
        private readonly ILogger _logger;

        public RequestAppService(
            IRequestService requestService,
            IExpertService expertService,
            ISkillService skillService,
            ICustomerService customerService,
            IProposalService proposalService,
            ILogger logger)
        {
            _requestService = requestService;
            _expertService = expertService;
            _skillService = skillService;
            _customerService = customerService;
            _proposalService = proposalService;
            _logger = logger;
        }

        public async Task<bool> CreateRequestAsync(CreateRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating request for CustomerId: {CustomerId}, SubHomeServiceId: {SubHomeServiceId}", dto.CustomerId, dto.SubHomeServiceId);
            return await _requestService.CreateAsync(dto, cancellationToken);
        }

        public async Task<List<RequestDto>> GetRequestsByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Getting requests for CustomerId: {CustomerId}", customerId);
            return await _requestService.GetRequestsByCustomerIdAsync(customerId, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating request with Id: {Id}", id);
            return await _requestService.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<RequestDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Getting request with Id: {Id}", id);
            return await _requestService.GetAsync(id, cancellationToken);
        }

        public async Task<List<RequestDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Getting all requests");
            return await _requestService.GetAllAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting request with Id: {Id}", id);
            return await _requestService.DeleteAsync(id, cancellationToken);
        }

        public async Task<List<RequestDto>> GetAvailableRequestsForExpertAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Getting available requests for ExpertId: {ExpertId}", expertId);

            try
            {
                var allRequests = await _requestService.GetAllAsync(cancellationToken);
                var pendingRequests = allRequests.Where(r => r.Status == RequestStatus.Pending && r.IsEnabled).ToList();

                var expert = await _expertService.GetByIdAsync(expertId, cancellationToken);
                if (expert == null)
                {
                    _logger.Warning("Expert not found for ExpertId: {ExpertId}", expertId);
                    return new List<RequestDto>();
                }

                var expertSkills = await _skillService.GetSkillsByExpertIdAsync(expertId, cancellationToken);
                if (expertSkills == null || !expertSkills.Any())
                {
                    _logger.Information("Expert with ID: {ExpertId} has no skills, returning all pending requests", expertId);
                    return pendingRequests;
                }

                var expertSubServiceIds = expertSkills.Select(s => s.SubHomeServiceId).Distinct().ToList();

                var availableRequests = pendingRequests.Where(r => expertSubServiceIds.Contains(r.SubHomeServiceId)).ToList();

                _logger.Information("Found {Count} available requests for ExpertId: {ExpertId}", availableRequests.Count, expertId);
                return availableRequests;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting available requests for ExpertId: {ExpertId}", expertId);
                return new List<RequestDto>();
            }
        }


        public async Task<List<RequestDto>> GetAvailableRequestsForExpertAsync(int expertId, string expertState, List<int> subHomeServiceIds, CancellationToken cancellationToken)
        {
            _logger.Information("Getting available requests for ExpertId: {ExpertId}, State: {State}, SubHomeServiceIds: {SubHomeServiceIds}",
                expertId, expertState, string.Join(",", subHomeServiceIds));

            try
            {
                var allRequests = await _requestService.GetAllAsync(cancellationToken);
                var pendingRequests = allRequests.Where(r => r.Status == RequestStatus.Pending && r.IsEnabled).ToList();

                if (pendingRequests.Count == 0)
                {
                    _logger.Information("No pending requests found");
                    return new List<RequestDto>();
                }

                var filteredByService = pendingRequests.Where(r => subHomeServiceIds.Contains(r.SubHomeServiceId)).ToList();

                if (filteredByService.Count == 0)
                {
                    _logger.Information("No requests found matching expert skills");
                    return new List<RequestDto>();
                }

                var customerIds = filteredByService.Select(r => r.CustomerId).Distinct().ToList();
                var customers = await _customerService.GetCustomersByIdsAsync(customerIds, cancellationToken);

                var sameStateCustomers = customers.Where(c => c.State == expertState).Select(c => c.Id).ToList();

                var result = filteredByService.Where(r => sameStateCustomers.Contains(r.CustomerId)).ToList();

                var expertProposals = await _proposalService.GetProposalsByExpertIdAsync(expertId, cancellationToken);
                var expertProposalRequestIds = expertProposals.Select(p => p.RequestId).ToList();

                result = result.Where(r => !expertProposalRequestIds.Contains(r.Id)).ToList();

                _logger.Information("Found {Count} available requests for ExpertId: {ExpertId}", result.Count, expertId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting available requests for ExpertId: {ExpertId}", expertId);
                return new List<RequestDto>();
            }
        }
    }

}