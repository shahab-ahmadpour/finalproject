using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Enums;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Endpoints.MVC.Models.Account;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using HomeService.Domain.AppServices.CustomerAppServices;

namespace App.Endpoints.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserAppService _userAppService;
        private readonly ICustomerAppService _customerAppService;
        private readonly Serilog.ILogger _logger;

        public AccountController(IUserAppService userAppService, ICustomerAppService customerAppService, Serilog.ILogger logger)
        {
            _userAppService = userAppService;
            _customerAppService = customerAppService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.Information("Displaying the registration form.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
        {
            _logger.Information("Received registration request for email: {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid registration model for email: {Email}", model.Email);
                return View(model);
            }

            var dto = new CreateAppUserDto
            {
                Email = model.Email,
                Password = model.Password,
                Role = Enum.TryParse<UserRole>(model.Role, out var parsedRole) ? parsedRole : UserRole.Customer,
                IsEnabled = true,
                IsConfirmed = false
            };

            var result = await _userAppService.RegisterAsync(dto, model.Password, cancellationToken);

            if (result.Succeeded)
            {
                _logger.Information("User with email {Email} registered successfully.", model.Email);
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.Warning("Failed to register user {Email}. Error: {Error}", model.Email, error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            _logger.Information("Displaying the login form.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to log in user with email: {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid login model for email: {Email}", model.Email);
                return View(model);
            }

            var result = await _userAppService.LoginAsync(model.Email, model.Password, model.RememberMe);

            if (result.Succeeded)
            {
                var user = await _userAppService.GetUserByEmailAsync(model.Email, cancellationToken);
                if (user != null)
                {
                    _logger.Information("Found user with AppUserId: {AppUserId}, Email: {Email}, Role: {Role}", user.Id, user.Email, user.Role);

                    if (!user.IsEnabled)
                    {
                        _logger.Warning("Login failed for email {Email}: User is disabled", model.Email);
                        ModelState.AddModelError(string.Empty, "حساب کاربری شما غیرفعال است. لطفاً با پشتیبانی تماس بگیرید.");
                        return View(model);
                    }

                    if (!user.IsConfirmed)
                    {
                        _logger.Warning("Login failed for email {Email}: User is not confirmed", model.Email);
                        ModelState.AddModelError(string.Empty, "حساب کاربری شما هنوز تأیید نشده است. لطفاً منتظر تأیید توسط مدیریت باشید.");
                        return View(model);
                    }

                    switch (user.Role)
                    {
                        case UserRole.Customer:
                            var customer = await _customerAppService.GetCustomerByAppUserIdAsync(user.Id, cancellationToken);
                            if (customer != null)
                            {
                                _logger.Information("Customer found with CustomerId: {CustomerId} for AppUserId: {AppUserId}", customer.Id, user.Id);
                                HttpContext.Session.Clear();
                                HttpContext.Session.SetInt32("UserId", user.Id); 
                                _logger.Information("Session set with UserId: {UserId} for email: {Email}", user.Id, model.Email);
                            }
                            else
                            {
                                _logger.Warning("No Customer found for AppUserId: {AppUserId}", user.Id);
                                ModelState.AddModelError(string.Empty, "حساب مشتری شما یافت نشد.");
                                return View(model);
                            }
                            break;

                        case UserRole.Admin:
                            _logger.Information("Admin login with AppUserId: {AppUserId}", user.Id);
                            HttpContext.Session.Clear();
                            HttpContext.Session.SetInt32("UserId", user.Id);
                            break;

                        case UserRole.Expert:
                            _logger.Information("Expert login with AppUserId: {AppUserId}", user.Id);
                            HttpContext.Session.Clear();
                            HttpContext.Session.SetInt32("UserId", user.Id);
                            break;

                        default:
                            _logger.Warning("Unknown role for email {Email}: {Role}", model.Email, user.Role);
                            HttpContext.Session.Clear();
                            ModelState.AddModelError(string.Empty, "نقش کاربر نامشخص است.");
                            return View(model);
                    }
                }

                return user.Role switch
                {
                    UserRole.Admin => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
                    UserRole.Customer => RedirectToAction("Dashboard", "Customer"),
                    UserRole.Expert => RedirectToAction("Dashboard", "Expert"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            _logger.Warning("Login failed for email {Email}: Invalid credentials", model.Email);
            ModelState.AddModelError(string.Empty, "ایمیل یا رمز عبور اشتباه است.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.Information("Attempting to log out the current user.");
            await _userAppService.LogoutAsync();
            HttpContext.Session.Clear();
            _logger.Information("User logged out successfully.");
            return RedirectToAction("Index", "Home");
        }
    }
}
