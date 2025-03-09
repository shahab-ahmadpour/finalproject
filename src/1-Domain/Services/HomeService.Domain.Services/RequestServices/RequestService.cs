using App.Domain.Core.DTO.Requests;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.RequestServices
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly ILogger _logger;

        public RequestService(IRequestRepository requestRepository, ILogger logger)
        {
            _requestRepository = requestRepository;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Creating new request for Customer ID: {CustomerId}", dto.CustomerId);
            try
            {
                if (dto.CustomerId == 0)
                {
                    _logger.Warning("Service: Invalid CustomerId (zero) for request creation.");
                    return false;
                }
                if (dto.SubHomeServiceId == 0)
                {
                    _logger.Warning("Service: Invalid SubHomeServiceId (zero) for request creation.");
                    return false;
                }

                var result = await _requestRepository.CreateAsync(dto, cancellationToken);
                _logger.Information("Service: CreateAsync returned: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to create request for CustomerId: {CustomerId}", dto.CustomerId);
                throw;
            }
        }

        public async Task<List<RequestDto>> GetRequestsByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching requests for CustomerId: {CustomerId}", customerId);
            var requests = await _requestRepository.GetRequestsByCustomerIdAsync(customerId, cancellationToken);
            _logger.Information("Service: Retrieved {Count} requests for CustomerId: {CustomerId}", requests.Count, customerId);
            return requests;
        }

        public async Task<bool> UpdateAsync(int id, UpdateRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating request with ID: {RequestId}", id);
            var result = await _requestRepository.UpdateAsync(id, dto, cancellationToken);
            _logger.Information("Service: UpdateAsync returned: {Result}", result);
            return result;
        }

        public async Task<RequestDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching request with ID: {RequestId}", id);
            var request = await _requestRepository.GetAsync(id, cancellationToken);
            _logger.Information("Service: Fetched request with ID: {RequestId}", id);
            return request;
        }

        public async Task<List<RequestDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all requests.");
            var requests = await _requestRepository.GetAllAsync(cancellationToken);
            _logger.Information("Service: Retrieved {Count} requests.", requests.Count);
            return requests;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Deleting (deactivating) request with ID: {RequestId}", id);
            var result = await _requestRepository.DeleteAsync(id, cancellationToken);
            _logger.Information("Service: DeleteAsync returned: {Result}", result);
            return result;
        }
    }
}
