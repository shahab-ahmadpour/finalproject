using App.Domain.Core.DTO.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IService
{
    public interface ICategoryService
    {
        Task<bool> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken);
        Task<CategoryDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<List<CategoryDto>> GetAllWithServicesAsync(CancellationToken cancellationToken);
        Task<List<CategoryListItemDto>> GetAllForDropdownAsync(CancellationToken cancellationToken);
    }
}
