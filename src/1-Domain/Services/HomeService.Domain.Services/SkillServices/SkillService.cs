using App.Domain.Core.Skills.Interfaces.IService;
using App.Domain.Core.Skills.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using App.Domain.Core.Skills.Entities;

namespace HomeService.Domain.Services.SkillServices
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;
        private readonly ILogger _logger;

        public SkillService(ISkillRepository skillRepository, ILogger logger)
        {
            _skillRepository = skillRepository;
            _logger = logger;
        }
        public async Task<List<Skill>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all skills");
            try
            {
                return await _skillRepository.GetAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error fetching all skills");
                throw;
            }
        }

        public async Task<Skill> GetByIdAsync(int skillId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching skill with ID: {SkillId}", skillId);
            try
            {
                var skill = await _skillRepository.GetByIdAsync(skillId, cancellationToken);
                if (skill == null)
                {
                    _logger.Warning("Service: Skill not found for ID: {SkillId}", skillId);
                }
                return skill;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error fetching skill with ID: {SkillId}", skillId);
                throw;
            }
        }

        public async Task AddAsync(Skill skill, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Adding new skill: {SkillName}", skill.Name);
            try
            {
                await _skillRepository.AddAsync(skill, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error adding skill: {SkillName}", skill.Name);
                throw;
            }
        }

        public async Task UpdateAsync(Skill skill, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating skill with ID: {SkillId}", skill.Id);
            try
            {
                await _skillRepository.UpdateAsync(skill, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error updating skill with ID: {SkillId}", skill.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int skillId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Deleting skill with ID: {SkillId}", skillId);
            try
            {
                await _skillRepository.DeleteAsync(skillId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error deleting skill with ID: {SkillId}", skillId);
                throw;
            }
        }

        public async Task<List<Skill>> GetSkillsByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching skills for ExpertId: {ExpertId}", expertId);
            try
            {
                var skills = await _skillRepository.GetSkillsByExpertIdAsync(expertId, cancellationToken);
                if (skills == null || !skills.Any())
                {
                    _logger.Warning("Service: No skills found for ExpertId: {ExpertId}", expertId);
                    return new List<Skill>();
                }
                _logger.Information("Service: Found {Count} skills for ExpertId: {ExpertId}", skills.Count, expertId);
                return skills;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error fetching skills for ExpertId: {ExpertId}", expertId);
                throw;
            }
        }

        public async Task<List<Skill>> GetSkillsByExpertIdAndSubHomeServiceIdAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching skills for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}", expertId, subHomeServiceId);
            try
            {
                var skills = await _skillRepository.GetSkillsByExpertIdAndSubHomeServiceIdAsync(expertId, subHomeServiceId, cancellationToken);
                if (skills == null || !skills.Any())
                {
                    _logger.Warning("Service: No skills found for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}", expertId, subHomeServiceId);
                    return new List<Skill>();
                }
                return skills;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error fetching skills for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}", expertId, subHomeServiceId);
                throw;
            }
        }

        public async Task<Skill> GetBySubHomeServiceIdAsync(int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching skill by SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
            try
            {
                var skill = await _skillRepository.GetBySubHomeServiceIdAsync(subHomeServiceId, cancellationToken);
                if (skill == null)
                {
                    _logger.Warning("Service: No skill found for SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
                }
                return skill;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error fetching skill for SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
                throw;
            }
        }
    }
}
