using App.Domain.Core.DTO.Categories;
using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IService;
using App.Domain.Core.Services.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Core.DTO.SubHomeServices;
using Microsoft.Extensions.Caching.Memory;

namespace HomeService.Domain.AppServices.HomeServiceAppServices
{
    public class HomeServiceAppService : IHomeServiceAppService
    {
        private readonly IHomeServiceService _homeServiceService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public HomeServiceAppService(
            IHomeServiceService homeServiceService,
            ICategoryService categoryService,
            ILogger logger,
            IMemoryCache memoryCache)
        {
            _homeServiceService = homeServiceService;
            _categoryService = categoryService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<bool> CreateAsync(CreateHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Creating HomeService with Name: {Name}", dto.Name);
            var result = await _homeServiceService.CreateAsync(dto, cancellationToken);
            _logger.Information("AppService: CreateAsync returned: {Result}", result);
            return result;
        }

        public async Task<bool> UpdateAsync(int id, UpdateHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating HomeService with Id: {Id}", id);
            var result = await _homeServiceService.UpdateAsync(id, dto, cancellationToken);
            _logger.Information("AppService: UpdateAsync returned: {Result}", result);
            return result;
        }

        public async Task<HomeServiceDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Getting HomeService with Id: {Id}", id);
            return await _homeServiceService.GetAsync(id, cancellationToken);
        }

        public async Task<List<HomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Getting all HomeServices");
            return await _homeServiceService.GetAllAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Deleting HomeService with Id: {Id}", id);
            var result = await _homeServiceService.DeleteAsync(id, cancellationToken);
            _logger.Information("AppService: DeleteAsync returned: {Result}", result);
            return result;
        }

        public async Task<UpdateHomeServiceDto> GetHomeServiceForEditAsync(int id, CancellationToken cancellationToken)
        {
            var homeService = await _homeServiceService.GetAsync(id, cancellationToken);
            if (homeService == null)
            {
                _logger.Warning("HomeService with Id: {Id} not found.", id);
                return null;
            }

            var dto = new UpdateHomeServiceDto
            {
                Id = homeService.Id,
                Name = homeService.Name,
                Description = homeService.Description,
                ImagePath = homeService.ImagePath.StartsWith("/") ? homeService.ImagePath : "/" + homeService.ImagePath
            };

            return dto;
        }
        public async Task<List<HomeServiceDto>> GetAllWithSubServicesAsync(CancellationToken cancellationToken)
        {
            return await _homeServiceService.GetAllWithSubServicesAsync(cancellationToken);
        }
        public async Task<List<CategoryListItemDto>> GetAllCategoriesForDropdownAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all categories for dropdown");
            var categories = await _categoryService.GetAllForDropdownAsync(cancellationToken);
            _logger.Information("AppService: Fetched {Count} categories for dropdown", categories?.Count ?? 0);
            return categories ?? new List<CategoryListItemDto>();
        }

        public async Task<List<HomeServiceDto>> GetAllHomeServicesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all HomeServices with their Categories in AppService layer.");
            string cacheKey = "AllHomeServices";

            if (!_memoryCache.TryGetValue(cacheKey, out List<HomeServiceDto> cachedHomeServices))
            {
                _logger.Information("HomeServices not found in cache, fetching from database");
                try
                {
                    var homeServices = await _homeServiceService.GetAllHomeServicesAsync(cancellationToken);
                    cachedHomeServices = homeServices.Select(hs => new HomeServiceDto
                    {
                        Id = hs.Id,
                        Name = hs.Name,
                        Description = hs.Description,
                        ImagePath = hs.ImagePath?.Replace("\\", "/") ?? "/images/homeservices/default.jpg",
                        CategoryId = hs.CategoryId,
                        IsActive = hs.IsActive,
                        SubHomeServices = hs.SubHomeServices?.Select(ss => new SubHomeServiceDto
                        {
                            Id = ss.Id,
                            Name = ss.Name,
                            Description = ss.Description,
                            Views = ss.Views,
                            BasePrice = ss.BasePrice,
                            HomeServiceId = ss.HomeServiceId,
                            IsActive = ss.IsActive
                        }).ToList() ?? new List<SubHomeServiceDto>()
                    }).ToList();

                    if (cachedHomeServices != null && cachedHomeServices.Any())
                    {
                        _logger.Information("Caching {HomeServiceCount} home services", cachedHomeServices.Count);
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                            SlidingExpiration = TimeSpan.FromMinutes(30)
                        };
                        _memoryCache.Set(cacheKey, cachedHomeServices, cacheOptions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to fetch HomeServices with Categories in AppService layer.");
                    throw;
                }
            }
            else
            {
                _logger.Information("HomeServices retrieved from cache, Count: {HomeServiceCount}", cachedHomeServices.Count);
            }
            return cachedHomeServices;
        }
    }
}
