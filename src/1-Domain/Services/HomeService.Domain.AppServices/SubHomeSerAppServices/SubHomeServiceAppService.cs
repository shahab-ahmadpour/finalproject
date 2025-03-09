using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IService;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.AppServices.SubHomeSerAppServices
{
    public class SubHomeServiceAppService : ISubHomeServiceAppService
    {
        private readonly ISubHomeServiceService _subHomeServiceService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public SubHomeServiceAppService(
            ISubHomeServiceService subHomeServiceService,
            ILogger logger,
            IMemoryCache memoryCache)
        {
            _subHomeServiceService = subHomeServiceService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<bool> CreateAsync(CreateSubHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Starting creation of SubHomeService with name: {Name}", dto.Name);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream, cancellationToken);
                }
                dto.ImagePath = "/uploads/" + fileName;
            }

            return await _subHomeServiceService.CreateAsync(dto, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, UpdateSubHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating SubHomeService with Id: {Id}", id);
            var result = await _subHomeServiceService.UpdateAsync(id, dto, cancellationToken);
            _logger.Information("AppService: UpdateAsync returned: {Result} for Id: {Id}", result, id);
            if (result)
                _logger.Information("AppService: SubHomeService with Id: {Id} updated successfully.", id);
            else
                _logger.Warning("AppService: Failed to update SubHomeService with Id: {Id}.", id);

            return result;
        }

        public async Task<SubHomeServiceDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching SubHomeService with Id: {Id}", id);
            var subHomeService = await _subHomeServiceService.GetAsync(id, cancellationToken);
            if (subHomeService != null)
                _logger.Information("AppService: SubHomeService with Id: {Id} fetched successfully.", id);
            else
                _logger.Warning("AppService: SubHomeService with Id: {Id} not found.", id);

            return subHomeService;
        }

        public async Task<List<SubHomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all SubHomeServices.");
            var subHomeServices = await _subHomeServiceService.GetAllAsync(cancellationToken);
            if (subHomeServices.Count > 0)
                _logger.Information("AppService: {Count} SubHomeServices fetched successfully.", subHomeServices.Count);
            else
                _logger.Warning("AppService: No SubHomeServices found.");

            return subHomeServices;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Deleting (disabling) SubHomeService with Id: {Id}", id);
            var result = await _subHomeServiceService.DeleteAsync(id, cancellationToken);
            if (result)
                _logger.Information("AppService: SubHomeService with Id: {Id} deleted successfully.", id);
            else
                _logger.Warning("AppService: Failed to delete SubHomeService with Id: {Id}.", id);

            return result;
        }

        public async Task<UpdateSubHomeServiceDto> GetSubHomeServiceForEditAsync(int id, CancellationToken cancellationToken)
        {
            var subHomeService = await _subHomeServiceService.GetAsync(id, cancellationToken);
            if (subHomeService == null)
            {
                _logger.Warning("SubHomeService with Id: {Id} not found.", id);
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
                IsActive = subHomeService.IsActive,
            };

            return dto;
        }

        public async Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching SubHomeServices in AppService layer.");
            string cacheKey = "AllSubHomeServices";

            if (!_memoryCache.TryGetValue(cacheKey, out List<SubHomeServiceListItemDto> cachedSubHomeServices))
            {
                _logger.Information("SubHomeServices not found in cache, fetching from database");
                try
                {
                    cachedSubHomeServices = await _subHomeServiceService.GetAllAsync(cancellationToken);
                    if (cachedSubHomeServices != null && cachedSubHomeServices.Any())
                    {
                        _logger.Information("Caching {SubHomeServiceCount} sub home services", cachedSubHomeServices.Count);
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                            SlidingExpiration = TimeSpan.FromMinutes(30)
                        };
                        _memoryCache.Set(cacheKey, cachedSubHomeServices, cacheOptions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to fetch SubHomeServices in AppService layer.");
                    throw;
                }
            }
            else
            {
                _logger.Information("SubHomeServices retrieved from cache, Count: {SubHomeServiceCount}", cachedSubHomeServices.Count);
            }
            return cachedSubHomeServices;
        }

        public async Task<SubHomeServiceListItemDto> GetSubHomeServiceByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching SubHomeService by Id: {Id}", id);
            try
            {
                var subHomeService = await _subHomeServiceService.GetSubHomeServiceByIdAsync(id, cancellationToken);
                if (subHomeService == null)
                {
                    _logger.Warning("AppService: SubHomeService not found for Id: {Id}", id);
                    return null;
                }
                _logger.Information("AppService: Found SubHomeService with Id: {Id}", id);
                return subHomeService;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AppService: Failed to fetch SubHomeService for Id: {Id}", id);
                throw;
            }
        }
        public async Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesByHomeServiceIdAsync(int homeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching sub-home services for HomeServiceId: {HomeServiceId}", homeServiceId);
            return await _subHomeServiceService.GetSubHomeServicesByHomeServiceIdAsync(homeServiceId, cancellationToken);
        }

        public async Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Incrementing view count for SubHomeServiceId: {Id}", id);

            try
            {
                var subService = await _subHomeServiceService.GetSubHomeServiceByIdAsync(id, cancellationToken);
                if (subService == null)
                {
                    _logger.Warning("SubHomeService not found for ID: {Id}", id);
                    return false;
                }

                var updateDto = new UpdateSubHomeServiceDto
                {
                    Id = id,
                    Name = subService.Name,
                    Description = subService.Description,
                    BasePrice = subService.BasePrice,
                    Views = subService.Views + 1,
                    ImagePath = subService.ImagePath,
                    IsActive = subService.IsActive
                };

                var result = await _subHomeServiceService.UpdateAsync(id, updateDto, cancellationToken);

                if (result)
                {
                    _logger.Information("Successfully incremented view count for SubHomeServiceId: {Id}", id);
                    return true;
                }
                else
                {
                    _logger.Warning("Failed to increment view count for SubHomeServiceId: {Id}", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error incrementing view count for SubHomeServiceId: {Id}", id);
                return false;
            }
        }
    }
}
