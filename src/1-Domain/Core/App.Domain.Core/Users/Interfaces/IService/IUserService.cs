using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Interfaces.IService
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken);
        Task<AppUserDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<AppUserDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<AppUserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
    }
}
