using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.DTO.Users.Experts;
using App.Domain.Core.Skills.Entities;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Domain.Core.Users.Interfaces.IService;
using Serilog;
using System.Threading;

namespace App.Domain.Core.Users.AppServices
{
    public class ExpertAppService : IExpertAppService
    {
        private readonly IExpertService _expertService;
        private readonly IUserAppService _userAppService;
        private readonly ILogger _logger;

        public ExpertAppService(
            IExpertService expertService,
            IUserAppService userAppService,
            ILogger logger)
        {
            _expertService = expertService;
            _userAppService = userAppService;
            _logger = logger;
        }

        public async Task<int?> GetAppUserIdByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching AppUserId for ExpertId: {ExpertId}", expertId);
            var expert = await _expertService.GetByIdAsync(expertId, cancellationToken);
            if (expert == null)
            {
                _logger.Warning("Expert not found for ExpertId: {ExpertId}", expertId);
                return null;
            }

            return expert.AppUserId;
        }

        public async Task<AppUserDto?> GetExpertUserByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching expert user for ExpertId: {ExpertId}", expertId);
            var expert = await _expertService.GetByIdAsync(expertId, cancellationToken);
            if (expert == null)
            {
                _logger.Warning("Expert not found for ExpertId: {ExpertId}", expertId);
                return null;
            }

            var user = await _userAppService.GetByIdAsync(expert.AppUserId, cancellationToken);
            if (user == null)
            {
                _logger.Warning("User not found for AppUserId: {AppUserId}", expert.AppUserId);
                return null;
            }

            return user;
        }

        public async Task<List<ExpertDto>> GetAllExpertsAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all experts");
            return await _expertService.GetAllAsync(cancellationToken);
        }

        public async Task<bool> CreateExpertAsync(CreateExpertDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating expert with AppUserId: {AppUserId}", dto.AppUserId);
            return await _expertService.CreateAsync(dto, cancellationToken);
        }

        public async Task<bool> UpdateExpertAsync(int id, UpdateExpertDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating expert with Id: {Id}", id);
            return await _expertService.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteExpertAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting expert with Id: {Id}", id);
            return await _expertService.DeleteAsync(id, cancellationToken);
        }

        public async Task<decimal> GetBalanceAsync(int expertId, CancellationToken cancellationToken)
        {
            return await _expertService.GetBalanceAsync(expertId, cancellationToken);
        }

        public async Task<bool> UpdateBalanceAsync(int expertId, decimal newBalance, CancellationToken cancellationToken)
        {
            return await _expertService.UpdateBalanceAsync(expertId, newBalance, cancellationToken);
        }

        public async Task<ExpertDto> GetByIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching expert with ID: {ExpertId}", expertId);
            var expertDto = await _expertService.GetByIdAsync(expertId, cancellationToken);

            if (expertDto == null)
            {
                _logger.Warning("Expert not found with ID: {ExpertId}", expertId);
                return null;
            }

            // Enhance with user data
            var userDto = await _userAppService.GetByIdAsync(expertDto.AppUserId, cancellationToken);
            if (userDto != null)
            {
                expertDto.FirstName = userDto.FirstName;
                expertDto.LastName = userDto.LastName;
                expertDto.Email = userDto.Email;
                expertDto.ProfilePicture = userDto.ProfilePicture;
                expertDto.AccountBalance = userDto.AccountBalance;
                expertDto.IsEnabled = userDto.IsEnabled;
                expertDto.IsConfirmed = userDto.IsConfirmed;
                expertDto.CreatedAt = userDto.CreatedAt;
                expertDto.Role = userDto.Role;
            }

            return expertDto;
        }

        public async Task<int> GetExpertIdByAppUserIdAsync(int appUserId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching ExpertId for AppUserId: {AppUserId}", appUserId);
            var expert = await _expertService.GetExpertByAppUserIdAsync(appUserId, cancellationToken);

            if (expert == null)
            {
                _logger.Warning("Expert not found for AppUserId: {AppUserId}", appUserId);
                return 0;
            }

            return expert.Id;
        }

        public async Task<EditExpertDto> GetEditExpertProfileAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching edit profile data for ExpertId: {ExpertId}", expertId);

            var expertDto = await GetByIdAsync(expertId, cancellationToken);
            if (expertDto == null)
            {
                _logger.Warning("Expert not found for ExpertId: {ExpertId}", expertId);
                return null;
            }

            var editDto = new EditExpertDto
            {
                AppUserId = expertDto.AppUserId,
                FirstName = expertDto.FirstName ?? "N/A",
                LastName = expertDto.LastName ?? "N/A",
                ProfilePicture = expertDto.ProfilePicture ?? "default.png",
                PhoneNumber = expertDto.PhoneNumber,
                Address = expertDto.Address,
                City = expertDto.City,
                State = expertDto.State
            };

            return editDto;
        }

        public async Task<bool> UpdateExpertProfileAsync(EditExpertDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating expert profile for AppUserId: {AppUserId}", dto.AppUserId);

            try
            {
                var updateUserDto = new UpdateAppUserDto
                {
                    Id = dto.AppUserId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    ProfilePicture = dto.ProfilePicture
                };

                var userResult = await _userAppService.UpdateAsync(dto.AppUserId, updateUserDto, cancellationToken);

                // Update expert info
                var expertId = await GetExpertIdByAppUserIdAsync(dto.AppUserId, cancellationToken);
                if (expertId <= 0)
                {
                    _logger.Warning("Could not find ExpertId for AppUserId: {AppUserId}", dto.AppUserId);
                    return false;
                }

                var updateExpertDto = new UpdateExpertDto
                {
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State
                };

                var expertResult = await _expertService.UpdateAsync(expertId, updateExpertDto, cancellationToken);

                return userResult && expertResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating expert profile for AppUserId: {AppUserId}", dto.AppUserId);
                return false;
            }
        }

        public async Task<bool> AddSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Calling AddSkillAsync for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}",
                expertId, subHomeServiceId);
            return await _expertService.AddSkillAsync(expertId, subHomeServiceId, cancellationToken);
        }

        public async Task<bool> RemoveSkillAsync(int expertId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Calling RemoveSkillAsync for ExpertId: {ExpertId} and SubHomeServiceId: {SubHomeServiceId}",
                expertId, subHomeServiceId);
            return await _expertService.RemoveSkillAsync(expertId, subHomeServiceId, cancellationToken);
        }
    }
}