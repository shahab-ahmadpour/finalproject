using App.Domain.Core.DTO.Requests;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Services
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public RequestRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // این متد را در RequestRepository جایگزین کنید

        public async Task<bool> CreateAsync(CreateRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating new request for CustomerId: {CustomerId}", dto.CustomerId);
            try
            {
                var request = new Request
                {
                    CustomerId = dto.CustomerId,
                    SubHomeServiceId = dto.SubHomeServiceId,
                    Description = dto.Description,
                    Deadline = DateTime.Now.AddDays(7),
                    ExecutionDate = dto.ExecutionDate,
                    Status = dto.Status,
                    CreatedAt = DateTime.UtcNow,
                    IsEnabled = true
                };

                if (dto.EnvironmentImagePaths != null && dto.EnvironmentImagePaths.Any())
                {

                    request.EnvironmentImagePath = string.Join(";", dto.EnvironmentImagePaths);
                }

                _dbContext.Requests.Add(request);
                var result = await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Request created successfully for CustomerId: {CustomerId}, Result: {Result}", dto.CustomerId, result > 0);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create request for CustomerId: {CustomerId}", dto.CustomerId);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating request with ID: {RequestId}", id);
            try
            {
                var request = await _dbContext.Requests.FindAsync(id);
                if (request == null)
                {
                    _logger.Warning("Request with ID: {RequestId} not found for update.", id);
                    return false;
                }

                request.Status = dto.Status;
                request.Deadline = dto.Deadline;
                request.ExecutionDate = dto.ExecutionDate;
                if (!string.IsNullOrEmpty(dto.EnvironmentImagePath))
                {
                    request.EnvironmentImagePath = dto.EnvironmentImagePath;
                }
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully updated request with ID: {RequestId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update request with ID: {RequestId}", id);
                return false;
            }
        }

        public async Task<RequestDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching request with ID: {RequestId}", id);
            try
            {
                var request = await _dbContext.Requests
                    .Where(r => r.Id == id && r.IsEnabled)
                    .Include(r => r.SubHomeService)
                    .Select(r => new RequestDto
                    {
                        Id = r.Id,
                        CustomerId = r.CustomerId,
                        SubHomeServiceId = r.SubHomeServiceId,
                        SubHomeServiceName = r.SubHomeService.Name,
                        Description = r.Description,
                        Status = r.Status,
                        Deadline = r.Deadline,
                        ExecutionDate = r.ExecutionDate,
                        CreatedAt = r.CreatedAt,
                        EnvironmentImagePath = r.EnvironmentImagePath,
                        IsEnabled = r.IsEnabled
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (request == null)
                {
                    _logger.Warning("Request with ID: {RequestId} not found", id);
                }
                else
                {
                    _logger.Information("Fetched request with ID: {RequestId}", id);
                }

                return request;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch request with ID: {RequestId}", id);
                throw;
            }
        }

        public async Task<List<RequestDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all requests.");
            try
            {
                var requests = await _dbContext.Requests
                    .Where(r => r.IsEnabled)
                    .Include(r => r.SubHomeService)
                    .Select(r => new RequestDto
                    {
                        Id = r.Id,
                        CustomerId = r.CustomerId,
                        SubHomeServiceId = r.SubHomeServiceId,
                        SubHomeServiceName = r.SubHomeService.Name,
                        Description = r.Description,
                        Status = r.Status,
                        Deadline = r.Deadline,
                        ExecutionDate = r.ExecutionDate,
                        CreatedAt = r.CreatedAt,
                        EnvironmentImagePath = r.EnvironmentImagePath,
                        IsEnabled = r.IsEnabled
                    })
                    .ToListAsync(cancellationToken);

                _logger.Information("Fetched {Count} requests.", requests.Count);
                return requests;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all requests.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting (deactivating) request with ID: {RequestId}", id);
            try
            {
                var request = await _dbContext.Requests.FindAsync(id);
                if (request == null)
                {
                    _logger.Warning("Request with ID: {RequestId} not found for deletion.", id);
                    return false;
                }

                request.IsEnabled = false;
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully deactivated request with ID: {RequestId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deactivate request with ID: {RequestId}", id);
                return false;
            }
        }

        public async Task<List<RequestDto>> GetRequestsByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching requests for CustomerId: {CustomerId}", customerId);
            try
            {
                var requests = await _dbContext.Requests
                    .Where(r => r.CustomerId == customerId && r.IsEnabled)
                    .Include(r => r.SubHomeService)
                    .Select(r => new RequestDto
                    {
                        Id = r.Id,
                        CustomerId = r.CustomerId,
                        SubHomeServiceId = r.SubHomeServiceId,
                        SubHomeServiceName = r.SubHomeService.Name,
                        Description = r.Description,
                        Status = r.Status,
                        Deadline = r.Deadline,
                        ExecutionDate = r.ExecutionDate,
                        CreatedAt = r.CreatedAt,
                        EnvironmentImagePath = r.EnvironmentImagePath,
                        IsEnabled = r.IsEnabled
                    })
                    .ToListAsync(cancellationToken);

                _logger.Information("Fetched {Count} requests for CustomerId: {CustomerId}", requests.Count, customerId);
                return requests;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch requests for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }
    }

}
