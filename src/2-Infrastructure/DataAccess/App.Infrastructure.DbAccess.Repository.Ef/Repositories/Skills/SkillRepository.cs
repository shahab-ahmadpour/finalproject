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
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public SkillRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Skill>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching all skills");
            return await _dbContext.Skills.ToListAsync(cancellationToken);
        }

        public async Task<Skill> GetByIdAsync(int skillId, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching skill with ID: {SkillId}", skillId);
            return await _dbContext.Skills.FindAsync(new object[] { skillId }, cancellationToken);
        }

        public async Task AddAsync(Skill skill, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Adding new skill: {SkillName}", skill.Name);
            await _dbContext.Skills.AddAsync(skill, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Skill skill, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Updating skill with ID: {SkillId}", skill.Id);
            _dbContext.Skills.Update(skill);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Deleting skill with ID: {SkillId}", id);
            var skill = await _dbContext.Skills.FindAsync(new object[] { id }, cancellationToken);
            if (skill != null)
            {
                _dbContext.Skills.Remove(skill);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<Skill>> GetSkillsByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching skills for ExpertId: {ExpertId}", expertId);
            try
            {
                var expertSkills = await _dbContext.ExpertSkills
                    .Include(es => es.Skill)
                    .ThenInclude(s => s.SubHomeService)
                    .Where(es => es.ExpertId == expertId)
                    .ToListAsync(cancellationToken);

                _logger.Information("ExpertSkills raw count: {Count}", expertSkills.Count);

                var skills = expertSkills.Select(es => es.Skill).ToList();

                if (skills == null || !skills.Any())
                {
                    _logger.Warning("Repository: No skills found for ExpertId: {ExpertId}", expertId);
                    return new List<Skill>();
                }

                _logger.Information("Repository: Found {Count} skills for ExpertId: {ExpertId}", skills.Count, expertId);
                return skills;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Repository: Error fetching skills for ExpertId: {ExpertId}", expertId);
                throw;
            }
        }

        public async Task<List<Skill>> GetSkillsByExpertIdAndSubHomeServiceIdAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching skills for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}",
                expertId, subHomeServiceId);

            try
            {
                var skills = await _dbContext.Skills
                    .Where(s => s.SubHomeServiceId == subHomeServiceId)
                    .ToListAsync(cancellationToken);

                if (skills == null || !skills.Any())
                {
                    _logger.Warning("No skills found for SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
                    return new List<Skill>();
                }

                var skillIds = skills.Select(s => s.Id).ToList();

                var expertSkills = await _dbContext.ExpertSkills
                    .Where(es => es.ExpertId == expertId && skillIds.Contains(es.SkillId))
                    .Include(es => es.Skill)
                    .Select(es => es.Skill)
                    .ToListAsync(cancellationToken);

                if (expertSkills == null || !expertSkills.Any())
                {
                    _logger.Warning("No skills found for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}",
                        expertId, subHomeServiceId);
                    return new List<Skill>();
                }

                _logger.Information("Found {Count} skills for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}",
                    expertSkills.Count, expertId, subHomeServiceId);

                return expertSkills;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching skills for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}",
                    expertId, subHomeServiceId);
                throw;
            }
        }

        public async Task<Skill> GetBySubHomeServiceIdAsync(int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Finding skill by SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);

            try
            {
                var skill = await _dbContext.Skills
                    .Where(s => s.SubHomeServiceId == subHomeServiceId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (skill == null)
                {
                    _logger.Information("No skill found for SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
                    return null;
                }

                _logger.Information("Found skill (ID: {SkillId}) for SubHomeServiceId: {SubHomeServiceId}",
                    skill.Id, subHomeServiceId);

                return skill;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding skill for SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
                throw;
            }
        }
    }

}