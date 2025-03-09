using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Interfaces.IRepository
{
    public interface IAdminUserRepository
    {
        Task<List<AppUserDto>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<AppUserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        Task<IdentityResult> CreateUserAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken);
        Task<bool> UpdateUserAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken);
        List<AppUser> GetAllUsers();
        Task<bool> ActivateUserAsync(int id, CancellationToken cancellationToken);
    }
}
