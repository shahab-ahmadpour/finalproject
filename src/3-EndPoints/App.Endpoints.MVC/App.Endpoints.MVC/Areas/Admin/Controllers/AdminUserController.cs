using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Endpoints.MVC.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly IAdminUserAppService _adminUserAppService;
        private readonly Serilog.ILogger _logger;

        public AdminUserController(IAdminUserAppService adminUserAppService, Serilog.ILogger logger)
        {
            _adminUserAppService = adminUserAppService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _adminUserAppService.GetAllUsersAsync(cancellationToken);
            return View(users);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var user = await _adminUserAppService.GetUserByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateAppUserDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateAppUserDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || dto.Role == null)
            {
                ModelState.AddModelError("", "ایمیل، رمز عبور و نقش اجباری هستند.");
                return View(dto);
            }

            var result = await _adminUserAppService.CreateUserAsync(dto, dto.Password, cancellationToken);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "کاربر با موفقیت ساخته شد.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _adminUserAppService.GetUserByIdAsync(id, CancellationToken.None);
            if (user == null)
            {
                _logger.Warning("User with ID: {Id} not found.", id);
                return NotFound();
            }
            var updateDto = new UpdateAppUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsConfirmed = user.IsConfirmed,
                IsEnabled = user.IsEnabled
            };
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateAppUserDto dto)
        {
            _logger.Information("Received Edit POST request for User with ID: {Id}, FirstName={FirstName}, IsEnabled={IsEnabled}",
                id, dto.FirstName, dto.IsEnabled);

            if (ModelState.IsValid)
            {
                var existingUser = await _adminUserAppService.GetUserByIdAsync(id, CancellationToken.None);
                if (existingUser == null)
                {
                    _logger.Warning("User with ID: {Id} not found for update.", id);
                    TempData["ErrorMessage"] = "کاربر یافت نشد.";
                    return RedirectToAction("Index");
                }

                var updateDto = new UpdateAppUserDto
                {
                    Id = id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Role = dto.Role,
                    IsEnabled = dto.IsEnabled,
                    IsConfirmed = existingUser.IsConfirmed 
                };

                var result = await _adminUserAppService.UpdateUserAsync(id, updateDto, CancellationToken.None);
                if (result)
                {
                    TempData["SuccessMessage"] = "اطلاعات با موفقیت ذخیره شد.";
                    _logger.Information("User with ID: {Id} updated successfully", id);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "خطا در ویرایش اطلاعات کاربر.";
                    _logger.Warning("Failed to update User with ID: {Id}", id);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.Warning("ModelState error: {Error}", error.ErrorMessage);
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching user for deletion confirmation with ID: {Id}", id);

            var user = await _adminUserAppService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                _logger.Warning("User with ID: {Id} not found.");
                TempData["ErrorMessage"] = "کاربر یافت نشد.";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting user with ID: {Id}", id);

            var result = await _adminUserAppService.DeleteUserAsync(id, cancellationToken);

            if (result)
            {
                TempData["SuccessMessage"] = "کاربر با موفقیت حذف شد.";
                _logger.Information("User with ID: {Id} deleted successfully.", id);
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در حذف کاربر.";
                _logger.Warning("Failed to delete user with ID: {Id}.", id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
        {
            var result = await _adminUserAppService.ActivateUserAsync(id, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "کاربر با موفقیت فعال شد.";
                _logger.Information("User with ID: {Id} activated successfully", id);
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در فعال‌سازی کاربر.";
                _logger.Warning("Failed to activate user with ID: {Id}", id);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to confirm user with ID: {Id}", id);

            var user = await _adminUserAppService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
            {
                _logger.Warning("User with ID: {Id} not found.", id);
                TempData["ErrorMessage"] = "کاربر یافت نشد.";
                return RedirectToAction("Index");
            }

            if (user.IsConfirmed)
            {
                _logger.Warning("User with ID: {Id} is already confirmed.", id);
                TempData["ErrorMessage"] = "این کاربر قبلاً تأیید شده است.";
                return RedirectToAction("Index");
            }

            var updateDto = new UpdateAppUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsEnabled = user.IsEnabled,
                IsConfirmed = true 
            };

            var result = await _adminUserAppService.UpdateUserAsync(id, updateDto, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "کاربر با موفقیت تأیید شد.";
                _logger.Information("User with ID: {Id} confirmed successfully.", id);
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در تأیید کاربر.";
                _logger.Warning("Failed to confirm user with ID: {Id}", id);
            }

            return RedirectToAction("Index");
        }
    }
}
