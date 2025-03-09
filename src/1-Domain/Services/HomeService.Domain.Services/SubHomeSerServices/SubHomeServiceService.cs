using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.SubHomeSerServices
{
    public class SubHomeServiceService : ISubHomeServiceService
    {
        private readonly ISubHomeServiceRepository _subHomeServiceRepository;
        private readonly ILogger _logger;

        public SubHomeServiceService(ISubHomeServiceRepository subHomeServiceRepository, ILogger logger)
        {
            _subHomeServiceRepository = subHomeServiceRepository;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateSubHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating SubHomeService with name: {Name}", dto.Name);

            var subHomeService = new SubHomeService
            {
                Name = dto.Name,
                Description = dto.Description,
                Views = dto.Views,
                BasePrice = dto.BasePrice,
                HomeServiceId = dto.HomeServiceId,
                IsActive = dto.IsActive,
                ImagePath = dto.ImagePath
            };

            var result = await _subHomeServiceRepository.CreateAsync(subHomeService, cancellationToken);
            if (result)
                _logger.Information("SubHomeService created successfully.");
            else
                _logger.Warning("Failed to create SubHomeService.");

            return result;
        }

        public async Task<bool> UpdateAsync(int id, UpdateSubHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating SubHomeService with Id: {Id}", id);
            var result = await _subHomeServiceRepository.UpdateAsync(id, dto, cancellationToken);
            if (result)
                _logger.Information("SubHomeService with Id: {Id} updated successfully.", id);
            else
                _logger.Warning("Failed to update SubHomeService with Id: {Id}.", id);

            return result;
        }

        public async Task<SubHomeServiceDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching SubHomeService with Id: {Id}", id);
            var subHomeService = await _subHomeServiceRepository.GetAsync(id, cancellationToken);
            if (subHomeService != null)
                _logger.Information("SubHomeService with Id: {Id} fetched successfully.", id);
            else
                _logger.Warning("SubHomeService with Id: {Id} not found.", id);

            return subHomeService;
        }

        public async Task<List<SubHomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all SubHomeServices in service layer.");
            try
            {
                var services = await _subHomeServiceRepository.GetAllAsync(cancellationToken);
                _logger.Information("Fetched {Count} SubHomeServices from repository.", services?.Count ?? 0);
                return services;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch SubHomeServices in service layer.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting (disabling) SubHomeService with Id: {Id}", id);
            var result = await _subHomeServiceRepository.DeleteAsync(id, cancellationToken);
            if (result)
                _logger.Information("SubHomeService with Id: {Id} deleted successfully.", id);
            else
                _logger.Warning("Failed to delete SubHomeService with Id: {Id}.", id);

            return result;
        }

        public async Task<List<SubHomeService>> GetAllServicesAsync(CancellationToken cancellationToken = default)
        {
            _logger.Information("Fetching all SubHomeServices in service layer.");
            try
            {
                var services = await _subHomeServiceRepository.GetAllServicesAsync(cancellationToken);
                _logger.Information("Fetched {Count} SubHomeServices from repository.", services?.Count ?? 0);
                return services;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all SubHomeServices in service layer.");
                throw;
            }
        }

        public async Task<SubHomeServiceListItemDto> GetSubHomeServiceByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching SubHomeService by Id: {Id}", id);
            try
            {
                var subHomeService = await _subHomeServiceRepository.GetSubHomeServiceByIdAsync(id, cancellationToken);
                if (subHomeService == null)
                {
                    _logger.Warning("Service: SubHomeService not found for Id: {Id}", id);
                    return null;
                }
                _logger.Information("Service: Found SubHomeService with Id: {Id}", id);
                return subHomeService;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch SubHomeService for Id: {Id}", id);
                throw;
            }
        }

        public async Task<UpdateSubHomeServiceDto> GetSubHomeServiceForEditAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching SubHomeService for edit with Id: {Id}", id);
            try
            {
                var subHomeService = await _subHomeServiceRepository.GetAsync(id, cancellationToken);
                if (subHomeService == null)
                {
                    _logger.Warning("Service: SubHomeService not found for edit with Id: {Id}", id);
                    return null;
                }

                var dto = new UpdateSubHomeServiceDto
                {
                    Id = subHomeService.Id,
                    Name = subHomeService.Name,
                    Description = subHomeService.Description,
                    BasePrice = subHomeService.BasePrice,
                    Views = subHomeService.Views,
                    ImagePath = subHomeService.ImagePath.StartsWith("/") ? subHomeService.ImagePath : "/" + subHomeService.ImagePath,
                    IsActive = subHomeService.IsActive
                };

                _logger.Information("Service: Prepared edit DTO for SubHomeService with Id: {Id}", id);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch SubHomeService for edit with Id: {Id}", id);
                throw;
            }
        }

        public async Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching SubHomeServices in service layer.");
            try
            {
                var services = await _subHomeServiceRepository.GetAllAsync(cancellationToken);
                _logger.Information("Service: Fetched {Count} SubHomeServices from repository.", services?.Count ?? 0);
                return services;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch SubHomeServices in service layer.");
                throw;
            }
        }
        public async Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesByHomeServiceIdAsync(int homeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching sub-home services for HomeServiceId: {HomeServiceId}", homeServiceId);
            return await _subHomeServiceRepository.GetSubHomeServicesByHomeServiceIdAsync(homeServiceId, cancellationToken);
        }
    }
}
