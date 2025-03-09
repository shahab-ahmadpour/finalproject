using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Enums;
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

namespace HomeService.Domain.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly ILogger _logger;

        public UserService(IAppUserRepository userRepository, ILogger logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken)
        {
            _logger.Information("Registering user with email: {Email}", dto.Email);
            return await _userRepository.CreateUserAsync(dto, password, cancellationToken);
        }

        public async Task<AppUserDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<AppUserDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _userRepository.GetAllAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken)
        {
            return await _userRepository.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            return await _userRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<AppUserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByEmailAsync(email, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
        {
            return await _userRepository.ExistsAsync(email, cancellationToken);
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            _logger.Information("Logging in user with email: {Email}", email);
            return await _userRepository.LoginAsync(email, password, rememberMe);
        }

        public async Task LogoutAsync()
        {
            _logger.Information("Logging out user.");
            await _userRepository.LogoutAsync();
        }
    }

}
