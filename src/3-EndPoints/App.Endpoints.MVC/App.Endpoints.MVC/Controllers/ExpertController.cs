using App.Domain.Core.DTO.City;
using App.Domain.Core.DTO.Orders;
using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.DTO.Users.Experts;
using App.Domain.Core.Enums;
using App.Domain.Core.Locations;
using App.Domain.Core.Locations.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Skills.Interfaces.IAppServices;
using App.Domain.Core.Users.Interfaces.IAppService;
using HomeService.Domain.AppServices.SkillAppServices;
using HomeService.Domain.AppServices.SubHomeSerAppServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace App.Endpoints.MVC.Controllers
{
    public class ExpertController : Controller
    {
        private readonly IExpertAppService _expertAppService;
        private readonly IProposalAppService _proposalAppService;
        private readonly IOrderAppService _orderAppService;
        private readonly ISubHomeServiceAppService _subHomeServiceAppService;
        private readonly IRequestAppService _requestAppService;
        private readonly ISkillAppService _skillAppService;
        private readonly ILocationAppService _locationAppService;
        private readonly Serilog.ILogger _logger;

        public ExpertController(
            IExpertAppService expertAppService,
            IProposalAppService proposalAppService,
            IOrderAppService orderAppService,
            ISubHomeServiceAppService subHomeServiceAppService,
            IRequestAppService requestAppService,
            ISkillAppService skillAppService,
            ILocationAppService locationAppService,
            Serilog.ILogger logger)
        {
            _expertAppService = expertAppService;
            _proposalAppService = proposalAppService;
            _orderAppService = orderAppService;
            _subHomeServiceAppService = subHomeServiceAppService;
            _requestAppService = requestAppService;
            _skillAppService = skillAppService;
            _locationAppService = locationAppService;
            _logger = logger;
        }

        private async Task<int?> GetExpertIdFromSession(CancellationToken cancellationToken)
        {
            var appUserId = HttpContext.Session.GetInt32("UserId");
            if (!appUserId.HasValue)
            {
                _logger.Warning("No UserId (AppUserId) in session, redirecting to Login");
                return null;
            }

            _logger.Information("Session UserId (AppUserId): {AppUserId}", appUserId.Value);

            try
            {
                var expertId = await _expertAppService.GetExpertIdByAppUserIdAsync(appUserId.Value, cancellationToken);
                if (expertId <= 0)
                {
                    _logger.Warning("Expert not found for AppUserId: {AppUserId}", appUserId.Value);
                    return null;
                }

                _logger.Information("Found Expert with ExpertId: {ExpertId} for AppUserId: {AppUserId}",
                    expertId, appUserId.Value);
                return expertId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting expert ID for AppUserId: {AppUserId}", appUserId.Value);
                return null;
            }
        }

        public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var expertDto = await _expertAppService.GetByIdAsync(expertId.Value, cancellationToken);
            if (expertDto == null)
            {
                _logger.Warning("Expert not found for ExpertId: {ExpertId}", expertId.Value);
                return RedirectToAction("Login", "Account");
            }

            var proposals = await _proposalAppService.GetProposalsByExpertIdAsync(expertId.Value, cancellationToken);
            var orders = await _orderAppService.GetOrdersByExpertIdAsync(expertId.Value, cancellationToken);

            ViewBag.Proposals = proposals;
            ViewBag.Orders = orders;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            _logger.Information("Dashboard loaded successfully for ExpertId: {ExpertId}", expertId.Value);
            return View(expertDto);
        }

        public async Task<IActionResult> EditProfile(CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.Information("EditProfile called with ExpertId: {ExpertId}", expertId.Value);

            var editDto = await _expertAppService.GetEditExpertProfileAsync(expertId.Value, cancellationToken);
            if (editDto == null)
            {
                _logger.Warning("Expert not found for ExpertId: {ExpertId}", expertId.Value);
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Provinces = await _locationAppService.GetAllProvincesAsync(cancellationToken);
            ViewBag.Cities = string.IsNullOrEmpty(editDto.State)
                ? new List<CityDto>()
                : await _locationAppService.GetCitiesByProvinceNameAsync(editDto.State, cancellationToken);

            var skills = await _skillAppService.GetSkillsByExpertIdAsync(expertId.Value, cancellationToken);
            _logger.Information("Skills loaded for ExpertId: {ExpertId}, Count: {Count}, Details: {@Skills}",
                expertId.Value, skills?.Count ?? 0, skills);
            ViewBag.ExpertSkills = skills;

            ViewBag.SubHomeServices = await _subHomeServiceAppService.GetAllAsync(cancellationToken);

            return View(editDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditExpertDto model, CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.ProfilePictureFile == null || model.ProfilePictureFile.Length == 0)
            {
                ModelState.Remove("ProfilePictureFile");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.Error("ModelState invalid. Errors: {Errors}", string.Join(", ", errors));

                ViewBag.Provinces = await _locationAppService.GetAllProvincesAsync(cancellationToken);
                ViewBag.Cities = string.IsNullOrEmpty(model.State)
                    ? new List<CityDto>()
                    : await _locationAppService.GetCitiesByProvinceNameAsync(model.State, cancellationToken);
                ViewBag.ExpertSkills = await _skillAppService.GetSkillsByExpertIdAsync(expertId.Value, cancellationToken);
                ViewBag.SubHomeServices = await _subHomeServiceAppService.GetAllAsync(cancellationToken);

                return View(model);
            }

            try
            {
                if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePictureFile.FileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePictureFile.CopyToAsync(stream);
                    }

                    model.ProfilePicture = $"/uploads/{fileName}";
                }
                else
                {
                    var currentExpert = await _expertAppService.GetByIdAsync(expertId.Value, cancellationToken);
                    model.ProfilePicture = currentExpert?.ProfilePicture ?? "default.png";
                }

                var result = await _expertAppService.UpdateExpertProfileAsync(model, cancellationToken);

                if (result)
                {
                    TempData["SuccessMessage"] = "پروفایل شما با موفقیت به‌روزرسانی شد!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["ErrorMessage"] = "خطا در به‌روزرسانی پروفایل. لطفاً دوباره تلاش کنید.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in EditProfile for ExpertId: {ExpertId}", expertId.Value);
                TempData["ErrorMessage"] = "خطای سیستمی در به‌روزرسانی پروفایل.";
            }

            ViewBag.Provinces = await _locationAppService.GetAllProvincesAsync(cancellationToken);
            ViewBag.Cities = string.IsNullOrEmpty(model.State)
                ? new List<CityDto>() // تغییر به CityDto
                : await _locationAppService.GetCitiesByProvinceNameAsync(model.State, cancellationToken);
            ViewBag.ExpertSkills = await _skillAppService.GetSkillsByExpertIdAsync(expertId.Value, cancellationToken);
            ViewBag.SubHomeServices = await _subHomeServiceAppService.GetAllAsync(cancellationToken);

            return View(model);
        }

        public async Task<IActionResult> Proposals(CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var proposals = await _proposalAppService.GetProposalsByExpertIdAsync(expertId.Value, cancellationToken);
            _logger.Information("Proposals loaded successfully for ExpertId: {ExpertId}, Count: {ProposalCount}",
                expertId.Value, proposals?.Count ?? 0);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(proposals);
        }

        public async Task<IActionResult> Orders(CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _orderAppService.GetOrdersByExpertIdAsync(expertId.Value, cancellationToken);
            _logger.Information("Orders loaded successfully for ExpertId: {ExpertId}, Count: {OrderCount}",
                expertId.Value, orders?.Count ?? 0);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int orderId, CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                _logger.Warning("No ExpertId in session, redirecting to Login");
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderAppService.GetAsync(orderId, cancellationToken);
            if (order == null)
            {
                _logger.Warning("Order not found for OrderId: {OrderId}", orderId);
                TempData["ErrorMessage"] = "سفارش یافت نشد.";
                return RedirectToAction("Orders");
            }

            if (order.ExpertId != expertId.Value)
            {
                _logger.Warning("Order {OrderId} does not belong to ExpertId: {ExpertId}", orderId, expertId.Value);
                TempData["ErrorMessage"] = "شما دسترسی به این سفارش ندارید.";
                return RedirectToAction("Orders");
            }

            _logger.Information("Order details loaded successfully for OrderId: {OrderId}, ExpertId: {ExpertId}",
                orderId, expertId.Value);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(order);
        }

        public async Task<IActionResult> AvailableRequests(CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var expert = await _expertAppService.GetByIdAsync(expertId.Value, cancellationToken);
            if (expert == null)
            {
                _logger.Warning("Expert not found for ExpertId: {ExpertId}", expertId.Value);
                TempData["ErrorMessage"] = "اطلاعات کارشناس یافت نشد.";
                return RedirectToAction("Dashboard");
            }

            if (string.IsNullOrEmpty(expert.State) || string.IsNullOrEmpty(expert.City))
            {
                _logger.Warning("Expert profile is incomplete for ExpertId: {ExpertId}", expertId.Value);
                TempData["ErrorMessage"] = "لطفاً اطلاعات پروفایل خود را با تکمیل استان و شهر کامل کنید.";
                return RedirectToAction("EditProfile");
            }

            var skills = await _skillAppService.GetSkillsByExpertIdAsync(expertId.Value, cancellationToken);
            if (skills == null || !skills.Any())
            {
                _logger.Warning("No skills found for ExpertId: {ExpertId}", expertId.Value);
                TempData["ErrorMessage"] = "شما هنوز مهارتی ثبت نکرده‌اید. لطفاً ابتدا مهارت‌های خود را در پروفایل ثبت کنید.";
                return RedirectToAction("EditProfile");
            }

            var subHomeServiceIds = skills.Select(s => s.SubHomeServiceId).Distinct().ToList();

            var requests = await _requestAppService.GetAvailableRequestsForExpertAsync(expertId.Value, expert.State, subHomeServiceIds, cancellationToken);

            _logger.Information("Available requests loaded successfully for ExpertId: {ExpertId}, Count: {RequestCount}",
                expertId.Value, requests?.Count ?? 0);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(requests);
        }


        public async Task<IActionResult> CreateProposal(int requestId, CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _requestAppService.GetAsync(requestId, cancellationToken);
            if (request == null)
            {
                _logger.Warning("Request not found for RequestId: {RequestId}", requestId);
                TempData["ErrorMessage"] = "درخواست یافت نشد.";
                return RedirectToAction("AvailableRequests");
            }

            if (request.Status != RequestStatus.Pending)
            {
                _logger.Warning("Cannot create proposal for request with status: {Status}", request.Status);
                TempData["ErrorMessage"] = "امکان ارائه پیشنهاد برای این درخواست وجود ندارد.";
                return RedirectToAction("AvailableRequests");
            }

            var existingProposals = await _proposalAppService.GetProposalsByRequestIdAsync(requestId, cancellationToken);
            if (existingProposals != null && existingProposals.Any(p => p.ExpertId == expertId.Value))
            {
                _logger.Warning("Expert {ExpertId} has already submitted a proposal for request {RequestId}", expertId.Value, requestId);
                TempData["ErrorMessage"] = "شما قبلاً برای این درخواست پیشنهاد ارائه کرده‌اید.";
                return RedirectToAction("AvailableRequests");
            }

            var expertSkills = await _skillAppService.GetSkillsByExpertIdAndSubHomeServiceIdAsync(expertId.Value, request.SubHomeServiceId, cancellationToken);
            if (expertSkills == null || !expertSkills.Any())
            {
                _logger.Warning("No skills found for ExpertId {ExpertId} and SubHomeServiceId {SubHomeServiceId}", expertId.Value, request.SubHomeServiceId);
                TempData["ErrorMessage"] = "شما مهارت لازم برای این درخواست را ندارید.";
                return RedirectToAction("AvailableRequests");
            }

            var model = new CreateProposalDto
            {
                ExpertId = expertId.Value,
                RequestId = requestId,
                ExecutionDate = request.ExecutionDate
            };

            ViewBag.Request = request;
            ViewBag.Skills = expertSkills;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProposal(CreateProposalDto model, CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var request = await _requestAppService.GetAsync(model.RequestId, cancellationToken);
                ViewBag.Request = request;
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                return View(model);
            }

            model.ExpertId = expertId.Value;
            model.Status = ProposalStatus.Pending;

            var result = await _proposalAppService.CreateProposalAsync(model, cancellationToken);
            if (result)
            {
                _logger.Information("Proposal created successfully for ExpertId: {ExpertId}, RequestId: {RequestId}",
                    expertId.Value, model.RequestId);
                TempData["SuccessMessage"] = "پیشنهاد شما با موفقیت ثبت شد!";
                return RedirectToAction("Proposals");
            }
            else
            {
                _logger.Warning("Failed to create proposal for ExpertId: {ExpertId}, RequestId: {RequestId}",
                    expertId.Value, model.RequestId);
                TempData["ErrorMessage"] = "خطا در ثبت پیشنهاد.";
                return RedirectToAction("AvailableRequests");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CompleteOrder(int orderId, CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderAppService.GetAsync(orderId, cancellationToken);
            if (order == null)
            {
                _logger.Warning("Order not found for OrderId: {OrderId}", orderId);
                TempData["ErrorMessage"] = "سفارش یافت نشد.";
                return RedirectToAction("Orders");
            }

            if (order.ExpertId != expertId.Value)
            {
                _logger.Warning("Order {OrderId} does not belong to ExpertId: {ExpertId}", orderId, expertId.Value);
                TempData["ErrorMessage"] = "شما دسترسی به این سفارش ندارید.";
                return RedirectToAction("Orders");
            }

            if (order.Status == RequestStatus.Completed)
            {
                _logger.Warning("Order {OrderId} is already completed", orderId);
                TempData["ErrorMessage"] = "این سفارش قبلاً تکمیل شده است.";
                return RedirectToAction("Orders");
            }

            try
            {
                var updateOrderDto = new UpdateOrderDto
                {
                    Status = RequestStatus.Completed,
                    CompletionDate = DateTime.Now,
                    PaymentStatus = order.PaymentStatus,
                    IsActive = order.IsActive
                };

                var result = await _orderAppService.UpdateAsync(orderId, updateOrderDto, cancellationToken);

                if (result)
                {
                    _logger.Information("Order {OrderId} completed successfully by ExpertId: {ExpertId}", orderId, expertId.Value);
                    TempData["SuccessMessage"] = "سفارش با موفقیت تکمیل شد.";
                }
                else
                {
                    _logger.Warning("Failed to complete order {OrderId} by ExpertId: {ExpertId}", orderId, expertId.Value);
                    TempData["ErrorMessage"] = "خطا در تکمیل سفارش.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error completing order {OrderId} by ExpertId: {ExpertId}", orderId, expertId.Value);
                TempData["ErrorMessage"] = "خطا در تکمیل سفارش.";
            }

            return RedirectToAction("Orders");
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByProvinceName(string provinceName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information("Fetching cities for Province: {ProvinceName}", provinceName);
                if (string.IsNullOrEmpty(provinceName))
                {
                    _logger.Warning("ProvinceName is empty, returning empty list");
                    return Json(new List<CityDto>());
                }

                var cities = await _locationAppService.GetCitiesByProvinceNameAsync(provinceName, cancellationToken);
                _logger.Information("Cities loaded for Province: {ProvinceName}, Count: {Count}, Details: {@Cities}",
                    provinceName, cities.Count, cities);
                return Json(cities);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching cities for Province: {ProvinceName}", provinceName);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill(int subHomeServiceId, CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return Json(new { success = false, message = "لطفاً وارد حساب کاربری خود شوید." });
            }

            try
            {
                var subHomeService = await _subHomeServiceAppService.GetAsync(subHomeServiceId, cancellationToken);
                if (subHomeService == null)
                {
                    return Json(new { success = false, message = "سرویس مورد نظر یافت نشد." });
                }

                var expertSkills = await _skillAppService.GetSkillsByExpertIdAsync(expertId.Value, cancellationToken);
                if (expertSkills != null && expertSkills.Any(s => s.SubHomeServiceId == subHomeServiceId))
                {
                    return Json(new { success = false, message = "این مهارت قبلاً به لیست مهارت‌های شما اضافه شده است." });
                }

                var result = await _expertAppService.AddSkillAsync(expertId.Value, subHomeServiceId, cancellationToken);

                if (result)
                {
                    return Json(new { success = true, message = "مهارت با موفقیت اضافه شد." });
                }
                else
                {
                    return Json(new { success = false, message = "خطا در افزودن مهارت." });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding skill for Expert: {ExpertId}, SubHomeService: {SubHomeServiceId}",
                    expertId.Value, subHomeServiceId);
                return Json(new { success = false, message = "خطای سیستمی در افزودن مهارت." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSkill(int subHomeServiceId, CancellationToken cancellationToken)
        {
            var expertId = await GetExpertIdFromSession(cancellationToken);
            if (!expertId.HasValue)
            {
                return Json(new { success = false, message = "لطفاً وارد حساب کاربری خود شوید." });
            }

            try
            {
                var result = await _expertAppService.RemoveSkillAsync(expertId.Value, subHomeServiceId, cancellationToken);
                if (result)
                {
                    _logger.Information("Skill removed successfully for Expert: {ExpertId}, SubHomeService: {SubHomeServiceId}",
                        expertId.Value, subHomeServiceId);
                    return Json(new { success = true, message = "مهارت با موفقیت حذف شد." });
                }
                else
                {
                    _logger.Warning("Failed to remove skill for Expert: {ExpertId}, SubHomeService: {SubHomeServiceId}",
                        expertId.Value, subHomeServiceId);
                    return Json(new { success = false, message = "خطا در حذف مهارت." });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error removing skill for Expert: {ExpertId}, SubHomeService: {SubHomeServiceId}",
                    expertId.Value, subHomeServiceId);
                return Json(new { success = false, message = "خطای سیستمی در حذف مهارت." });
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _logger.Information("User logged out, clearing session for AppUserId: {AppUserId}", HttpContext.Session.GetInt32("UserId"));
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}