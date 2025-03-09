using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Enums;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Users
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger _logger;

        public AdminUserRepository(AppDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<List<AppUserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all users for admin management.");
            return await _dbContext.Users
                .Select(u => new AppUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    IsConfirmed = u.IsConfirmed,
                    IsEnabled = u.IsEnabled,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<AppUserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching user by ID: {Id}", id);
            return await _dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new AppUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    IsEnabled = u.IsEnabled,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IdentityResult> CreateUserAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken)
        {
            _logger.Information("Creating a new user via admin panel with Email: {Email}", dto.Email);

            // ساخت کاربر اصلی (AppUser)
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName ?? string.Empty,
                LastName = dto.LastName ?? string.Empty,  
                Role = dto.Role,
                IsEnabled = dto.IsEnabled,
                IsConfirmed = false, 
                CreatedAt = DateTime.UtcNow,
                ProfilePicture = "default.png", 
                AccountBalance = 0 
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                _logger.Warning("Failed to create user {Email}. Errors: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result;
            }

            var roleName = dto.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<int> { Name = roleName, NormalizedName = roleName.ToUpper() });
            }
            await _userManager.AddToRoleAsync(user, roleName);

            switch (dto.Role)
            {
                case UserRole.Customer:
                    var customer = new Customer
                    {
                        AppUserId = user.Id,
                        AppUser = user,
                        PhoneNumber = string.Empty,
                        Address = string.Empty,
                        City = string.Empty,
                        State = string.Empty
                    };
                    await _dbContext.Customers.AddAsync(customer, cancellationToken);
                    _logger.Information("Customer created for AppUserId: {AppUserId}", user.Id);
                    break;

                case UserRole.Expert:
                    var expert = new Expert
                    {
                        AppUserId = user.Id,
                        AppUser = user,
                        PhoneNumber = string.Empty,
                        Address = string.Empty,
                        City = string.Empty,
                        State = string.Empty
                    };
                    await _dbContext.Experts.AddAsync(expert, cancellationToken);
                    _logger.Information("Expert created for AppUserId: {AppUserId}", user.Id);
                    break;

                case UserRole.Admin:
                    break;

                default:
                    _logger.Warning("No specific entity created for role: {Role}", dto.Role);
                    break;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.Information("User with Email: {Email} created successfully with Role: {Role}", dto.Email, dto.Role);
            return result;
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating user with ID: {Id}", id);

            var user = await _dbContext.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
            {
                _logger.Warning("User not found with ID: {Id}", id);
                return false;
            }

            _logger.Information("Before update - User: FirstName={FirstName}, IsConfirmed={IsConfirmed}, IsEnabled={IsEnabled}",
                user.FirstName, user.IsConfirmed, user.IsEnabled);

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;

            if (Enum.IsDefined(typeof(UserRole), dto.Role))
            {
                user.Role = dto.Role;
            }
            else
            {
                _logger.Warning("Invalid role provided for user with ID: {Id}", id);
                return false;
            }

            if (user.IsConfirmed && !dto.IsConfirmed)
            {
                _logger.Information("Cannot set IsConfirmed to false for user with ID: {Id} as it was already true", id);
            }
            else
            {
                user.IsConfirmed = dto.IsConfirmed;
            }

            user.IsEnabled = dto.IsEnabled;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var result = await _userManager.ChangePasswordAsync(user, user.PasswordHash, dto.Password);
                if (!result.Succeeded)
                {
                    _logger.Warning("Failed to update password for user with ID: {Id}", id);
                    return false;
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("After update - User: FirstName={FirstName}, IsConfirmed={IsConfirmed}, IsEnabled={IsEnabled}",
                    user.FirstName, user.IsConfirmed, user.IsEnabled);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save changes for user with ID: {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Soft deleting (disabling) user with ID: {Id}", id);

            var user = await _dbContext.Users
                     .Where(u => u.Id == id)
                     .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                _logger.Warning("User not found with ID: {Id}", id);
                return false;
            }

            user.IsEnabled = false;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.Information("User with ID: {Id} was disabled successfully", id);
            return true;  
        }

        public async Task<bool> ActivateUserAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Activating user with ID: {Id}", id);
            var user = await _dbContext.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
            {
                _logger.Warning("User not found with ID: {Id}", id);
                return false;
            }

            if (user.IsEnabled)
            {
                _logger.Information("User with ID: {Id} is already enabled", id);
                return true;
            }

            user.IsEnabled = true;
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("User with ID: {Id} activated successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to activate user with ID: {Id}", id);
                return false;
            }
        }

        public List<AppUser> GetAllUsers()
        {
            return _dbContext.Users.ToList();
        }
    }
}
