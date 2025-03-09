using App.Domain.Core.DTO.Skills;
using App.Domain.Core.Skills.Entities;
using App.Domain.Core.Skills.Interfaces.IAppServices;
using App.Domain.Core.Skills.Interfaces.IService;
using Serilog;

namespace HomeService.Domain.AppServices.SkillAppServices
{
    public class SkillAppService : ISkillAppService
    {
        private readonly ISkillService _skillService;
        private readonly ILogger _logger;

        public SkillAppService(ISkillService skillService, ILogger logger)
        {
            _skillService = skillService;
            _logger = logger;
        }

        public async Task<List<SkillDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all skills");
            var skills = await _skillService.GetAllAsync(cancellationToken);
            return skills.Select(s => MapToDto(s)).ToList();
        }

        public async Task<SkillDto> GetByIdAsync(int skillId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching skill with ID: {SkillId}", skillId);
            var skill = await _skillService.GetByIdAsync(skillId, cancellationToken);
            return skill != null ? MapToDto(skill) : null;
        }

        public async Task AddAsync(SkillDto skillDto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Adding new skill: {SkillName}", skillDto.Name);
            var skill = new Skill
            {
                Name = skillDto.Name,
                SubHomeServiceId = skillDto.SubHomeServiceId
            };
            await _skillService.AddAsync(skill, cancellationToken);
        }

        public async Task UpdateAsync(SkillDto skillDto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating skill with ID: {SkillId}", skillDto.Id);
            var skill = new Skill
            {
                Id = skillDto.Id,
                Name = skillDto.Name,
                SubHomeServiceId = skillDto.SubHomeServiceId
            };
            await _skillService.UpdateAsync(skill, cancellationToken);
        }

        public async Task DeleteAsync(int skillId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Deleting skill with ID: {SkillId}", skillId);
            await _skillService.DeleteAsync(skillId, cancellationToken);
        }

        public async Task<List<SkillDto>> GetSkillsByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching skills for ExpertId: {ExpertId}", expertId);
            var skills = await _skillService.GetSkillsByExpertIdAsync(expertId, cancellationToken);
            return skills.Select(s => MapToDto(s)).ToList();
        }

        public async Task<List<SkillDto>> GetSkillsByExpertIdAndSubHomeServiceIdAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching skills for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}", expertId, subHomeServiceId);
            var skills = await _skillService.GetSkillsByExpertIdAndSubHomeServiceIdAsync(expertId, subHomeServiceId, cancellationToken);
            return skills.Select(s => MapToDto(s)).ToList();
        }

        public async Task<SkillDto> GetBySubHomeServiceIdAsync(int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching skill by SubHomeServiceId: {SubHomeServiceId}", subHomeServiceId);
            var skill = await _skillService.GetBySubHomeServiceIdAsync(subHomeServiceId, cancellationToken);
            return skill != null ? MapToDto(skill) : null;
        }

        private SkillDto MapToDto(Skill skill)
        {
            return new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name,
                SubHomeServiceId = skill.SubHomeServiceId,
                SubHomeServiceName = skill.SubHomeService?.Name // اگه SubHomeService لود نشده، باید توی Service لود بشه
            };
        }
    }
}