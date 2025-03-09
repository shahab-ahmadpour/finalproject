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
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger _logger;

        public AppUserRepository(AppDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole<int>> roleManager, ILogger logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<AppUserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching user with ID: {Id}", id);
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new AppUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    ProfilePicture = u.ProfilePicture,
                    AccountBalance = u.AccountBalance,
                    IsEnabled = u.IsEnabled,
                    IsConfirmed = u.IsConfirmed,
                    CreatedAt = u.CreatedAt,
                    Role = u.Role
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<AppUserDto> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching user with Email: {Email}", email);
            return await _dbContext.Users
                .Where(u => u.Email == email)
                .Select(u => new AppUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    ProfilePicture = u.ProfilePicture,
                    AccountBalance = u.AccountBalance,
                    IsEnabled = u.IsEnabled,
                    IsConfirmed = u.IsConfirmed,
                    CreatedAt = u.CreatedAt,
                    Role = u.Role
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<AppUserDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all users");
            return await _dbContext.Users
                .Select(u => new AppUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    ProfilePicture = u.ProfilePicture,
                    AccountBalance = u.AccountBalance,
                    IsEnabled = u.IsEnabled,
                    IsConfirmed = u.IsConfirmed,
                    CreatedAt = u.CreatedAt,
                    Role = u.Role
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IdentityResult> CreateUserAsync(CreateAppUserDto dto, string password, CancellationToken cancellationToken)
        {
            _logger.Information("Creating a new user with Email: {Email}", dto.Email);
            var user = new AppUser
            {
                FirstName = dto.FirstName ?? "Default",
                LastName = dto.LastName ?? "Default",
                Email = dto.Email,
                UserName = dto.Email,
                ProfilePicture = dto.ProfilePicture ?? "default.png",
                AccountBalance = dto.AccountBalance,
                IsEnabled = dto.IsEnabled,
                IsConfirmed = dto.IsConfirmed,
                CreatedAt = DateTime.UtcNow,
                Role = dto.Role
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

            _logger.Information("User created successfully with Email: {Email}", dto.Email);
            return result;
        }

        public async Task<bool> UpdateAsync(int id, UpdateAppUserDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating user with ID: {Id}", id);
            var user = await _dbContext.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
            {
                _logger.Warning("User with ID: {Id} not found", id);
                return false;
            }

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.ProfilePicture = dto.ProfilePicture ?? user.ProfilePicture;
            user.AccountBalance = dto.AccountBalance ?? user.AccountBalance;

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.Information("User with ID: {Id} updated successfully, ProfilePicture: {ProfilePicture}, AccountBalance: {AccountBalance}", id, user.ProfilePicture, user.AccountBalance);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting user with ID: {Id}", id);
            var user = await _dbContext.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
            {
                _logger.Warning("User with ID: {Id} not found for deletion", id);
                return false;
            }

            user.IsEnabled = false;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.Information("User with ID: {Id} deleted successfully", id);
            return true;
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
        {
            _logger.Information("Checking if user with Email: {Email} exists", email);
            return await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            _logger.Information("Attempting to log in user with email: {Email}", email);
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            _logger.Information("Logging out the current user.");
            await _signInManager.SignOutAsync();
        }
    }

}
