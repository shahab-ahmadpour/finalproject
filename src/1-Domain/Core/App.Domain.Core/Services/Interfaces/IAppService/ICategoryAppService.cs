using App.Domain.Core.DTO.Categories;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IAppService
{
    public interface ICategoryAppService
    {
        Task<bool> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken);
        Task<CategoryDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<List<CategoryDto>> GetAllWithServicesAsync(CancellationToken cancellationToken);
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken);
    }
}
