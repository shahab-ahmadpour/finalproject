using App.Domain.Core.DTO.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Skills.Interfaces.IAppServices
{
    public interface ISkillAppService
    {
        Task<List<SkillDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<SkillDto> GetByIdAsync(int skillId, CancellationToken cancellationToken);
        Task AddAsync(SkillDto skillDto, CancellationToken cancellationToken);
        Task UpdateAsync(SkillDto skillDto, CancellationToken cancellationToken);
        Task DeleteAsync(int skillId, CancellationToken cancellationToken);

        Task<List<SkillDto>> GetSkillsByExpertIdAsync(int expertId, CancellationToken cancellationToken);
        Task<List<SkillDto>> GetSkillsByExpertIdAndSubHomeServiceIdAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken);
        Task<SkillDto> GetBySubHomeServiceIdAsync(int subHomeServiceId, CancellationToken cancellationToken);
    }
}
