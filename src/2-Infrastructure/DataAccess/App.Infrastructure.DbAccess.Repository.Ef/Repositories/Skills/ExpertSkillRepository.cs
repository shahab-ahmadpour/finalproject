using App.Domain.Core.Skills.Entities;
using App.Domain.Core.Skills.Interfaces;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Skills
{
    public class ExpertSkillRepository : IExpertSkillRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public ExpertSkillRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<ExpertSkill>> GetByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching expert skills for ExpertId: {ExpertId}", expertId);
            try
            {
                var expertSkills = await _dbContext.ExpertSkills
                    .Where(es => es.ExpertId == expertId)
                    .ToListAsync(cancellationToken);

                _logger.Information("Fetched {Count} expert skills for ExpertId: {ExpertId}", expertSkills.Count, expertId);
                return expertSkills;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch expert skills for ExpertId: {ExpertId}", expertId);
                throw;
            }
        }

        public async Task AddAsync(ExpertSkill expertSkill, CancellationToken cancellationToken)
        {
            _logger.Information("Adding new expert skill for ExpertId: {ExpertId}, SkillId: {SkillId}", expertSkill.ExpertId, expertSkill.SkillId);
            try
            {
                if (expertSkill == null)
                    throw new ArgumentNullException(nameof(expertSkill));

                await _dbContext.ExpertSkills.AddAsync(expertSkill, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.Information("Successfully added expert skill for ExpertId: {ExpertId}, SkillId: {SkillId}", expertSkill.ExpertId, expertSkill.SkillId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add expert skill for ExpertId: {ExpertId}, SkillId: {SkillId}", expertSkill.ExpertId, expertSkill.SkillId);
                throw;
            }
        }

        public async Task DeleteAsync(int expertId, int skillId, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting expert skill for ExpertId: {ExpertId}, SkillId: {SkillId}", expertId, skillId);
            try
            {
                var expertSkill = await _dbContext.ExpertSkills
                    .FirstOrDefaultAsync(es => es.ExpertId == expertId && es.SkillId == skillId, cancellationToken);

                if (expertSkill != null)
                {
                    _dbContext.ExpertSkills.Remove(expertSkill);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _logger.Information("Successfully deleted expert skill for ExpertId: {ExpertId}, SkillId: {SkillId}", expertId, skillId);
                }
                else
                {
                    _logger.Warning("Expert skill not found for ExpertId: {ExpertId}, SkillId: {SkillId}", expertId, skillId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete expert skill for ExpertId: {ExpertId}, SkillId: {SkillId}", expertId, skillId);
                throw;
            }
        }
    }
}
