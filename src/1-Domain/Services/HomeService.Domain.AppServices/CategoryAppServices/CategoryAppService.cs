using App.Domain.Core.DTO.Categories;
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

namespace HomeService.Domain.AppServices.CategoryAppServices
{
    public class CategoryAppService : ICategoryAppService
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public CategoryAppService(
            ICategoryService categoryService,
            ILogger logger,
            IMemoryCache memoryCache)
        {
            _categoryService = categoryService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<bool> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Creating new category with Name: {Name}", dto.Name);
            var result = await _categoryService.CreateAsync(dto, cancellationToken);
            _logger.Information("AppService: CreateAsync returned: {Result}", result);
            return result;
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating category with Id: {Id}", id);
            var result = await _categoryService.UpdateAsync(id, dto, cancellationToken);
            _logger.Information("AppService: UpdateAsync returned: {Result}", result);
            return result;
        }

        public async Task<CategoryDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Getting category with Id: {Id}", id);
            return await _categoryService.GetAsync(id, cancellationToken);
        }

        public async Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Getting all categories");
            return await _categoryService.GetAllAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Deleting category with Id: {Id}", id);
            var result = await _categoryService.DeleteAsync(id, cancellationToken);
            _logger.Information("AppService: DeleteAsync returned: {Result}", result);
            return result;
        }

        public async Task<List<CategoryDto>> GetAllWithServicesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all categories with services");
            return await _categoryService.GetAllWithServicesAsync(cancellationToken);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all Categories in AppService layer.");
            string cacheKey = "AllCategories";

            if (!_memoryCache.TryGetValue(cacheKey, out List<CategoryDto> cachedCategories))
            {
                _logger.Information("Categories not found in cache, fetching from database");
                try
                {
                    cachedCategories = await _categoryService.GetAllAsync(cancellationToken);
                    if (cachedCategories != null && cachedCategories.Any())
                    {
                        _logger.Information("Caching {CategoryCount} categories", cachedCategories.Count);
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                            SlidingExpiration = TimeSpan.FromMinutes(30)
                        };
                        _memoryCache.Set(cacheKey, cachedCategories, cacheOptions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to fetch Categories in AppService layer.");
                    throw;
                }
            }
            else
            {
                _logger.Information("Categories retrieved from cache, Count: {CategoryCount}", cachedCategories.Count);
            }
            return cachedCategories;
        }
    }
}
