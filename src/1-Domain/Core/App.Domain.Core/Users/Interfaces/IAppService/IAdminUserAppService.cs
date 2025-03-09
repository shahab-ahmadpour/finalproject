using App.Domain.Core.DTO.Users.AppUsers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Interfaces.IAppService
{
    public interface IAdminUserAppService
    {
        Task<List<AppUserDto>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<AppUserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        Task<IdentityResult> CreateUserAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken);
        Task<bool> UpdateUserAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken);
        Task<bool> ActivateUserAsync(int id, CancellationToken cancellationToken);
    }
}
