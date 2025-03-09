using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.DTO.Users.Experts;
using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Interfaces.IRepository
{
    public interface IExpertRepository
    {
        Task<ExpertDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<ExpertDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> CreateAsync(CreateExpertDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateExpertDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<decimal> GetBalanceAsync(int expertId, CancellationToken cancellationToken);
        Task<bool> UpdateBalanceAsync(int expertId, decimal newBalance, CancellationToken cancellationToken);

        Task<Expert> GetByAppUserIdAsync(int appUserId, CancellationToken cancellationToken);
    }
}