using App.Domain.Core.DTO.Users.Experts;
using App.Domain.Core.Users.Entities;

namespace App.Domain.Core.Users.Interfaces.IService
{
    public interface IExpertService
    {
        Task<ExpertDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<ExpertDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> CreateAsync(CreateExpertDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateExpertDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<decimal> GetBalanceAsync(int expertId, CancellationToken cancellationToken);
        Task<bool> UpdateBalanceAsync(int expertId, decimal newBalance, CancellationToken cancellationToken);

        Task<Expert> GetExpertByAppUserIdAsync(int appUserId, CancellationToken cancellationToken);

        Task<bool> AddSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken);
        Task<bool> RemoveSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken);
    }
}