using App.Domain.Core.DTO.Categories;
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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public CategoryRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating new category: {Name}, ImagePath={ImagePath}", dto.Name, dto.ImagePath);
            try
            {
                var category = new Category
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ImagePath = dto.ImagePath ?? "/images/Categories/default.jpg",
                    IsActive = true
                };

                await _dbContext.Categories.AddAsync(category, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully created category: {Name}", dto.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create category: {Name}", dto.Name);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating category with ID: {Id}, Name={Name}, IsActive={IsActive}", id, dto.Name, dto.IsActive);
            try
            {
                var category = await _dbContext.Categories.FindAsync(new object[] { id }, cancellationToken);
                if (category == null)
                {
                    _logger.Warning("Category with ID: {Id} not found", id);
                    return false;
                }

                _logger.Information("Before update - Category: Name={Name}, IsActive={IsActive}", category.Name, category.IsActive);
                category.Name = dto.Name;
                category.Description = dto.Description;
                category.ImagePath = dto.ImagePath;
                category.IsActive = dto.IsActive;

                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully updated category with ID: {Id}, New Name={Name}, New IsActive={IsActive}", id, category.Name, category.IsActive);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update category with ID: {Id}", id);
                return false;
            }
        }

        public async Task<CategoryDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching category with ID: {Id}", id);
            try
            {
                var category = await _dbContext.Categories
                    .Where(c => c.Id == id)
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        ImagePath = c.ImagePath.Replace("\\", "/"),
                        IsActive = c.IsActive
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (category == null)
                {
                    _logger.Warning("Category with ID: {Id} not found", id);
                }
                else
                {
                    _logger.Information("Fetched category with ID: {Id}, ImagePath: {ImagePath}", id, category.ImagePath);
                }

                return category;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch category with ID: {Id}", id);
                throw;
            }
        }

        public async Task<List<CategoryListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all categories.");
            try
            {
                var categories = await _dbContext.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new CategoryListItemDto
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToListAsync(cancellationToken);

                _logger.Information("Fetched {Count} categories.", categories.Count);
                return categories;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all categories.");
                throw;
            }
        }
        public async Task<List<CategoryDto>> GetAllDetailedAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all categories (detailed list).");
            try
            {
                var categories = await _dbContext.Categories
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        ImagePath = c.ImagePath.Replace("\\", "/"),
                        IsActive = c.IsActive
                    })
                    .ToListAsync(cancellationToken);

                _logger.Information("Fetched {Count} detailed categories.", categories.Count);
                return categories;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all detailed categories.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting (deactivating) category with ID: {Id}", id);
            try
            {
                var category = await _dbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.Warning("Category with ID: {Id} not found for deletion.", id);
                    return false;
                }

                category.IsActive = false;
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully deactivated category with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deactivate category with ID: {Id}", id);
                return false;
            }
        }
        public async Task<List<Category>> GetAllWithServicesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Categories
                .Include(c => c.HomeServices)
                .ThenInclude(hs => hs.SubHomeServices.Where(ss => ss.IsActive))
                .ToListAsync(cancellationToken);
        }
    }


}
