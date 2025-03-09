using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Domain.Core.Users.Interfaces.IService;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.AdminServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly ILogger _logger;

        public AdminUserService(IAdminUserRepository adminUserRepository, ILogger logger)
        {
            _adminUserRepository = adminUserRepository;
            _logger = logger;
        }

        public async Task<List<AppUserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all users for admin management.");
            return await _adminUserRepository.GetAllUsersAsync(cancellationToken);
        }

        public async Task<AppUserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching user by ID: {Id}", id);
            return await _adminUserRepository.GetUserByIdAsync(id, cancellationToken);
        }

        public async Task<IdentityResult> CreateUserAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken)
        {
            _logger.Information("Creating a new user via admin panel with Email: {Email}", dto.Email);
            return await _adminUserRepository.CreateUserAsync(dto, password, cancellationToken);
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating user with ID: {Id}", id);
            return await _adminUserRepository.UpdateUserAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting (disabling) user with ID: {Id}", id);
            return await _adminUserRepository.DeleteUserAsync(id, cancellationToken);
        }
        public List<AppUser> GetAllUsers()
        {
            return _adminUserRepository.GetAllUsers();
        }
        public async Task<bool> ActivateUserAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Activating user with ID: {Id}", id);
            return await _adminUserRepository.ActivateUserAsync(id, cancellationToken);
        }
    }
}
