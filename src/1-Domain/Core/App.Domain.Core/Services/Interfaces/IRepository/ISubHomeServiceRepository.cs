using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IRepository
{
    public interface ISubHomeServiceRepository
    {
        Task<bool> CreateAsync(SubHomeService subHomeService, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateSubHomeServiceDto dto, CancellationToken cancellationToken);
        Task<SubHomeServiceDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<SubHomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<List<SubHomeService>> GetAllServicesAsync(CancellationToken cancellationToken = default);
        Task<SubHomeServiceListItemDto> GetSubHomeServiceByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesByHomeServiceIdAsync(int homeServiceId, CancellationToken cancellationToken);

    }
}
