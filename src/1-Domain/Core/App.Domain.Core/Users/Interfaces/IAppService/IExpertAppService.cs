using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.DTO.Users.Experts;

namespace App.Domain.Core.Users.Interfaces.IAppService
{
    public interface IExpertAppService
    {
        Task<int?> GetAppUserIdByExpertIdAsync(int expertId, CancellationToken cancellationToken);
        Task<AppUserDto?> GetExpertUserByExpertIdAsync(int expertId, CancellationToken cancellationToken);
        Task<List<ExpertDto>> GetAllExpertsAsync(CancellationToken cancellationToken);
        Task<bool> CreateExpertAsync(CreateExpertDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateExpertAsync(int id, UpdateExpertDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteExpertAsync(int id, CancellationToken cancellationToken);
        Task<decimal> GetBalanceAsync(int expertId, CancellationToken cancellationToken);
        Task<bool> UpdateBalanceAsync(int expertId, decimal newBalance, CancellationToken cancellationToken);

        Task<ExpertDto> GetByIdAsync(int expertId, CancellationToken cancellationToken);
        Task<int> GetExpertIdByAppUserIdAsync(int appUserId, CancellationToken cancellationToken);
        Task<EditExpertDto> GetEditExpertProfileAsync(int expertId, CancellationToken cancellationToken);
        Task<bool> UpdateExpertProfileAsync(EditExpertDto dto, CancellationToken cancellationToken);

        Task<bool> AddSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken);
        Task<bool> RemoveSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken);
    }
}