using App.Domain.Core.DTO.Categories;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IRepository
{
    public interface ICategoryRepository
    {
        Task<bool> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken);
        Task<CategoryDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<CategoryListItemDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<CategoryDto>> GetAllDetailedAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<List<Category>> GetAllWithServicesAsync(CancellationToken cancellationToken);
    }
}
