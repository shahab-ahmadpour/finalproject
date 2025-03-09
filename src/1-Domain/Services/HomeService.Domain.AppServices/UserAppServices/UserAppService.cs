using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Interfaces.IService;
using App.Domain.Core.Users.Interfaces.IAppService;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.AppServices.UserAppServices
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public UserAppService(IUserService userService, ILogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken)
        {
            _logger.Information("Registering user with email: {Email}", dto.Email);
            return await _userService.RegisterAsync(dto, password, cancellationToken);
        }

        public async Task<AppUserDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _userService.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<AppUserDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken)
        {
            return await _userService.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            return await _userService.DeleteAsync(id, cancellationToken);
        }

        public async Task<AppUserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _userService.GetUserByEmailAsync(email, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
        {
            return await _userService.ExistsAsync(email, cancellationToken);
        }
        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            _logger.Information("Logging in user with email: {Email}", email);
            return await _userService.LoginAsync(email, password, rememberMe);
        }

        public async Task LogoutAsync()
        {
            _logger.Information("Logging out user.");
            await _userService.LogoutAsync();
        }
    }
}
