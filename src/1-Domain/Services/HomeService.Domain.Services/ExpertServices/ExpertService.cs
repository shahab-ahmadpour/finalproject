using App.Domain.Core.DTO.Users.Experts;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Skills.Entities;
using App.Domain.Core.Skills.Interfaces;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Domain.Core.Users.Interfaces.IService;
using Serilog;
using System.Threading;

namespace App.Domain.Core.Users.Services
{
    public class ExpertService : IExpertService
    {
        private readonly IExpertRepository _expertRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IExpertSkillRepository _expertSkillRepository;
        private readonly ISubHomeServiceRepository _subHomeServiceRepository;
        private readonly ILogger _logger;

        public ExpertService(IExpertRepository expertRepository,  ILogger logger, IExpertSkillRepository expertSkillRepository, ISkillRepository skillRepository, ISubHomeServiceRepository subHomeServiceRepository )
        {
            _expertRepository = expertRepository;
            _logger = logger;
            _expertSkillRepository = expertSkillRepository;
            _subHomeServiceRepository = subHomeServiceRepository;
            _skillRepository = skillRepository;
        }

        public async Task<ExpertDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching Expert with ID: {Id}", id);
            return await _expertRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<ExpertDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all experts");
            return await _expertRepository.GetAllAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(CreateExpertDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Creating Expert with AppUserId: {AppUserId}", dto.AppUserId);
            return await _expertRepository.CreateAsync(dto, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, UpdateExpertDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating Expert with ID: {Id}", id);
            return await _expertRepository.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Deleting Expert with ID: {Id}", id);
            return await _expertRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<decimal> GetBalanceAsync(int expertId, CancellationToken cancellationToken)
        {
            return await _expertRepository.GetBalanceAsync(expertId, cancellationToken);
        }

        public async Task<bool> UpdateBalanceAsync(int expertId, decimal newBalance, CancellationToken cancellationToken)
        {
            return await _expertRepository.UpdateBalanceAsync(expertId, newBalance, cancellationToken);
        }

        public async Task<Expert> GetExpertByAppUserIdAsync(int appUserId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching Expert by AppUserId: {AppUserId}", appUserId);
            try
            {
                var expert = await _expertRepository.GetByAppUserIdAsync(appUserId, cancellationToken);
                if (expert == null)
                {
                    _logger.Warning("Service: No Expert found for AppUserId: {AppUserId}", appUserId);
                    return null;
                }

                _logger.Information("Service: Found Expert with Id: {ExpertId} for AppUserId: {AppUserId}", expert.Id, appUserId);
                return expert;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch Expert for AppUserId: {AppUserId}", appUserId);
                throw;
            }
        }
        public async Task<bool> AddSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Adding skill for SubHomeServiceId: {SubHomeServiceId} to ExpertId: {ExpertId}",
                subHomeServiceId, expertId);

            try
            {
                var skill = await _skillRepository.GetBySubHomeServiceIdAsync(subHomeServiceId, cancellationToken);

                if (skill == null)
                {
                    var subHomeService = await _subHomeServiceRepository.GetAsync(subHomeServiceId, cancellationToken);
                    if (subHomeService == null)
                    {
                        _logger.Warning("SubHomeService not found for ID: {SubHomeServiceId}", subHomeServiceId);
                        return false;
                    }

                    skill = new Skill
                    {
                        Name = subHomeService.Name,
                        SubHomeServiceId = subHomeServiceId
                    };

                    await _skillRepository.AddAsync(skill, cancellationToken);
                    _logger.Information("Created new skill for SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
                }

                var expertSkill = new ExpertSkill
                {
                    ExpertId = expertId,
                    SkillId = skill.Id
                };

                await _expertSkillRepository.AddAsync(expertSkill, cancellationToken);
                _logger.Information("Added skill {SkillId} to expert {ExpertId}", skill.Id, expertId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding skill for SubHomeServiceId: {SubHomeServiceId} to ExpertId: {ExpertId}",
                    subHomeServiceId, expertId);
                return false;
            }
        }

        public async Task<bool> RemoveSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Removing skill for SubHomeServiceId: {SubHomeServiceId} from ExpertId: {ExpertId}",
                subHomeServiceId, expertId);

            try
            {
                var skill = await _skillRepository.GetBySubHomeServiceIdAsync(subHomeServiceId, cancellationToken);

                if (skill == null)
                {
                    _logger.Warning("Skill not found for SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
                    return false;
                }

                await _expertSkillRepository.DeleteAsync(expertId, skill.Id, cancellationToken);
                _logger.Information("Removed skill {SkillId} from expert {ExpertId}", skill.Id, expertId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error removing skill for SubHomeServiceId: {SubHomeServiceId} from ExpertId: {ExpertId}",
                    subHomeServiceId, expertId);
                return false;
            }
        }
    }
}