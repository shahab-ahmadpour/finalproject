using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IAppService
{
    public interface ISubHomeServiceAppService
    {
        Task<bool> CreateAsync(CreateSubHomeServiceDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateSubHomeServiceDto dto, CancellationToken cancellationToken);
        Task<SubHomeServiceDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<SubHomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<UpdateSubHomeServiceDto> GetSubHomeServiceForEditAsync(int id, CancellationToken cancellationToken);
        Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesAsync(CancellationToken cancellationToken);
        Task<SubHomeServiceListItemDto> GetSubHomeServiceByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesByHomeServiceIdAsync(int homeServiceId, CancellationToken cancellationToken);

        Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken);
    }
}
