using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IService
{
    public interface IHomeServiceService
    {
        Task<bool> CreateAsync(CreateHomeServiceDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateHomeServiceDto dto, CancellationToken cancellationToken);
        Task<HomeServiceDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<HomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<List<HomeServiceDto>> GetAllWithSubServicesAsync(CancellationToken cancellationToken);
        Task<List<HomeService>> GetAllHomeServicesAsync(CancellationToken cancellationToken);
    }
}
