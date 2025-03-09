using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Domain.Core.Users.Interfaces.IService;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.AppServices.AdminAppServices
{
    public class AdminUserAppService : IAdminUserAppService
    {
        private readonly IAdminUserService _adminUserService;
        private readonly ILogger _logger;

        public AdminUserAppService(IAdminUserService adminUserService, ILogger logger)
        {
            _adminUserService = adminUserService;
            _logger = logger;
        }

        public async Task<List<AppUserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all users for admin panel.");
            return await _adminUserService.GetAllUsersAsync(cancellationToken);
        }

        public async Task<AppUserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching user by ID: {Id}", id);
            return await _adminUserService.GetUserByIdAsync(id, cancellationToken);
        }

        public async Task<IdentityResult> CreateUserAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken)
        {
            _logger.Information("Creating new user via admin panel with Email: {Email}", dto.Email);
            return await _adminUserService.CreateUserAsync(dto, password, cancellationToken);
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating user with ID: {Id}", id);
            return await _adminUserService.UpdateUserAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting (disabling) user with ID: {Id}", id);
            return await _adminUserService.DeleteUserAsync(id, cancellationToken);
        }
        public async Task<bool> ActivateUserAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Activating user with ID: {Id}", id);
            return await _adminUserService.ActivateUserAsync(id, cancellationToken);
        }
    }
}
