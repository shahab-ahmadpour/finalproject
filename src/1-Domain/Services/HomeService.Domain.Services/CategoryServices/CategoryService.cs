using App.Domain.Core.DTO.Categories;
using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Getting all categories");
            return await _categoryRepository.GetAllDetailedAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Creating new category with Name: {Name}", dto.Name);
            var result = await _categoryRepository.CreateAsync(dto, cancellationToken);
            _logger.Information("Service: CreateAsync returned: {Result}", result);
            return result;
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating category with Id: {Id}, Name={Name}, IsActive={IsActive}", id, dto.Name, dto.IsActive);
            var result = await _categoryRepository.UpdateAsync(id, dto, cancellationToken);
            _logger.Information("Service: UpdateAsync returned: {Result}", result);
            return result;
        }

        public async Task<CategoryDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Getting category with Id: {Id}", id);
            return await _categoryRepository.GetAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Deleting category with Id: {Id}", id);
            var result = await _categoryRepository.DeleteAsync(id, cancellationToken);
            _logger.Information("Service: DeleteAsync returned: {Result}", result);
            return result;
        }

        public async Task<List<CategoryDto>> GetAllWithServicesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all categories with services");
            var categories = await _categoryRepository.GetAllWithServicesAsync(cancellationToken);
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImagePath = c.ImagePath?.Replace("\\", "/") ?? "/images/categories/default.jpg",
                HomeServices = c.HomeServices.Select(hs => new HomeServiceDto
                {
                    Id = hs.Id,
                    Name = hs.Name,
                    ImagePath = hs.ImagePath?.Replace("\\", "/") ?? "/images/homeservices/default.jpg",
                    SubHomeServices = hs.SubHomeServices.Select(ss => new SubHomeServiceDto
                    {
                        Id = ss.Id,
                        Name = ss.Name,
                        BasePrice = ss.BasePrice,
                        HomeServiceId = ss.HomeServiceId,
                        IsActive = ss.IsActive
                    }).ToList()
                }).ToList()
            }).ToList();
        }

        public async Task<List<CategoryListItemDto>> GetAllForDropdownAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all categories for dropdown");
            return await _categoryRepository.GetAllAsync(cancellationToken);
        }
    }
}
