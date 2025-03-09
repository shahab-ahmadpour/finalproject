using App.Domain.Core.Skills.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Skills.Interfaces
{
    public interface IExpertSkillRepository
    {
        Task<List<ExpertSkill>> GetByExpertIdAsync(int expertId, CancellationToken cancellationToken);
        Task AddAsync(ExpertSkill expertSkill, CancellationToken cancellationToken);
        Task DeleteAsync(int expertId, int skillId, CancellationToken cancellationToken);
    }
}
