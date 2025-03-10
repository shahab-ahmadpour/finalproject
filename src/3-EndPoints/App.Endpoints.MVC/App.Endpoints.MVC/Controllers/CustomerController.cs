using App.Domain.Core.DTO.Categories;
using App.Domain.Core.DTO.City;
using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.DTO.Requests;
using App.Domain.Core.DTO.Reviews;
using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.DTO.Transactions;
using App.Domain.Core.DTO.Users.Customers;
using App.Domain.Core.Enums;
using App.Domain.Core.Locations.Interfaces.IAppService;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Transactions.Interfaces.IAppService;
using App.Domain.Core.Users.AppServices;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Endpoints.MVC.Models;
using HomeService.Domain.AppServices.CategoryAppServices;
using HomeService.Domain.AppServices.HomeServiceAppServices;
using HomeService.Domain.AppServices.ReviewAppServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace App.Endpoints.MVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly IOrderAppService _orderAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IHomeServiceAppService _homeServiceAppService;
        private readonly IRequestAppService _requestAppService;
        private readonly ISubHomeServiceAppService _subHomeServiceAppService;
        private readonly IProposalAppService _proposalAppService;
        private readonly ITransactionAppService _transactionAppService;
        private readonly IUserAppService _userAppService;
        private readonly IExpertAppService _expertAppService;
        private readonly IReviewAppService _reviewAppService;
        private readonly ILocationAppService _locationAppService;
        private readonly Serilog.ILogger _logger;

        public CustomerController(
            ICustomerAppService customerAppService,
            IOrderAppService orderAppService,
            ICategoryAppService categoryAppService,
            IHomeServiceAppService homeServiceAppService,
            IRequestAppService requestAppService,
            ISubHomeServiceAppService subHomeServiceAppService,
            IProposalAppService proposalAppService,
            ITransactionAppService transactionAppService,
            IUserAppService userAppService,
            IExpertAppService expertAppService,
            IReviewAppService reviewAppService,
            ILocationAppService locationAppService,
            Serilog.ILogger logger)
        {
            _customerAppService = customerAppService;
            _orderAppService = orderAppService;
            _categoryAppService = categoryAppService;
            _homeServiceAppService = homeServiceAppService;
            _requestAppService = requestAppService;
            _subHomeServiceAppService = subHomeServiceAppService;
            _proposalAppService = proposalAppService;
            _transactionAppService = transactionAppService;
            _userAppService = userAppService;
            _expertAppService = expertAppService;
            _reviewAppService = reviewAppService;
            _locationAppService = locationAppService;
            _logger = logger;
        }

        private async Task<int?> GetCustomerIdFromSession(CancellationToken cancellationToken)
        {
            var appUserId = HttpContext.Session.GetInt32("UserId");
            if (!appUserId.HasValue)
            {
                _logger.Warning("No UserId (AppUserId) in session, redirecting to Login");
                return null;
            }

            _logger.Information("Session UserId (AppUserId): {AppUserId}", appUserId.Value);

            var customer = await _customerAppService.GetCustomerByAppUserIdAsync(appUserId.Value, cancellationToken);
            if (customer == null)
            {
                _logger.Warning("Customer not found for AppUserId: {AppUserId}", appUserId.Value);
                return null;
            }

            _logger.Information("Found Customer with CustomerId: {CustomerId} for AppUserId: {AppUserId}, PhoneNumber: {PhoneNumber}",
                customer.Id, appUserId.Value, customer.PhoneNumber);
            return customer.Id;
        }

        [HttpGet]
        public async Task<IActionResult> ServiceHierarchy(CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = await _categoryAppService.GetAllCategoriesAsync(cancellationToken);
            var homeServices = await _homeServiceAppService.GetAllHomeServicesAsync(cancellationToken);
            var subHomeServices = await _subHomeServiceAppService.GetSubHomeServicesAsync(cancellationToken);

            _logger.Information("Raw data - Categories: {CatCount}, HomeServices: {HomeCount}, SubHomeServices: {SubCount}",
                categories?.Count ?? 0, homeServices?.Count ?? 0, subHomeServices?.Count ?? 0);

            var model = new ServiceHierarchyViewModel
            {
                Categories = categories ?? new List<CategoryDto>(),
                HomeServicesByCategory = homeServices?.GroupBy(hs => hs.CategoryId)
                    .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<HomeServiceDto>>(),
                SubHomeServicesByHomeService = subHomeServices?.GroupBy(s => s.HomeServiceId)
                    .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<SubHomeServiceListItemDto>>()
            };

            _logger.Information("Model data - Categories: {CatCount}, HomeServicesByCategory: {HomeCount}, SubHomeServicesByHomeService keys: {SubKeys}",
                model.Categories.Count, model.HomeServicesByCategory.Count, string.Join(", ", model.SubHomeServicesByHomeService.Keys));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubHomeServices(int homeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching sub-home services for HomeServiceId: {HomeServiceId}", homeServiceId);
            try
            {
                var subHomeServices = await _subHomeServiceAppService.GetSubHomeServicesByHomeServiceIdAsync(homeServiceId, cancellationToken);
                if (subHomeServices == null || !subHomeServices.Any())
                {
                    _logger.Warning("No sub-home services found for HomeServiceId: {HomeServiceId}", homeServiceId);
                    return Json(new { success = false, message = "هیچ زیرسرویسی پیدا نشد." });
                }
                _logger.Information("Found {Count} sub-home services for HomeServiceId: {HomeServiceId}", subHomeServices.Count, homeServiceId);
                return Json(new { success = true, data = subHomeServices });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch sub-home services for HomeServiceId: {HomeServiceId}", homeServiceId);
                return Json(new { success = false, message = "خطا در بارگذاری زیرسرویس‌ها." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Create(int subHomeServiceId, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var customerDto = await _customerAppService.GetByCustomerIdAsync(customerId.Value, cancellationToken);
            if (customerDto == null)
            {
                _logger.Warning("Customer not found for CustomerId: {CustomerId}", customerId.Value);
                return RedirectToAction("Login", "Account");
            }

            var subHomeService = await _subHomeServiceAppService.GetSubHomeServiceByIdAsync(subHomeServiceId, cancellationToken);
            if (subHomeService == null)
            {
                _logger.Warning("SubHomeService not found for Id: {SubHomeServiceId}", subHomeServiceId);
                return RedirectToAction("ServiceHierarchy");
            }

            var homeServiceId = subHomeService.HomeServiceId;
            ViewBag.HomeServiceId = homeServiceId;

            var model = new CreateRequestDto
            {
                CustomerId = customerId.Value,
                SubHomeServiceId = subHomeServiceId,
                SubHomeServiceName = subHomeService.Name,
                Status = RequestStatus.Pending,
                ExecutionDate = DateTime.Now.AddDays(1)
            };

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRequestDto model, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            model.CustomerId = customerId.Value;

            if (!ModelState.IsValid)
            {
                var subHomeService = await _subHomeServiceAppService.GetSubHomeServiceByIdAsync(model.SubHomeServiceId, cancellationToken);
                if (subHomeService != null)
                {
                    ViewBag.HomeServiceId = subHomeService.HomeServiceId;
                    model.SubHomeServiceName = subHomeService.Name;
                }

                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ExecutionTime))
            {
                var timeParts = model.ExecutionTime.Split(':');
                if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int hour) && int.TryParse(timeParts[1], out int minute))
                {
                    model.ExecutionDate = model.ExecutionDate.Date.AddHours(hour).AddMinutes(minute);
                }
            }

            if (model.EnvironmentImages != null && model.EnvironmentImages.Any())
            {
                foreach (var image in model.EnvironmentImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        try
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            model.EnvironmentImagePaths.Add($"/uploads/{fileName}");
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Failed to upload image for CustomerId: {CustomerId}", customerId.Value);
                        }
                    }
                }
            }

            model.Status = RequestStatus.Pending;
            var result = await _requestAppService.CreateRequestAsync(model, cancellationToken);

            if (result)
            {
                _logger.Information("Request created successfully for CustomerId: {CustomerId}", customerId.Value);
                TempData["SuccessMessage"] = "سفارش شما با موفقیت ثبت شد!";
                return RedirectToAction("Dashboard");
            }
            else
            {
                _logger.Warning("Failed to create request for CustomerId: {CustomerId}", customerId.Value);
                ModelState.AddModelError("", "خطا در ثبت درخواست. لطفاً دوباره تلاش کنید.");

                var subHomeService = await _subHomeServiceAppService.GetSubHomeServiceByIdAsync(model.SubHomeServiceId, cancellationToken);
                if (subHomeService != null)
                {
                    ViewBag.HomeServiceId = subHomeService.HomeServiceId;
                    model.SubHomeServiceName = subHomeService.Name;
                }

                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var customerDto = await _customerAppService.GetByCustomerIdAsync(customerId.Value, cancellationToken);
            if (customerDto == null)
            {
                _logger.Warning("Customer not found for CustomerId: {CustomerId}", customerId.Value);
                return RedirectToAction("Login", "Account");
            }

            var orders = await _customerAppService.GetOrdersByCustomerIdAsync(customerId.Value, cancellationToken);
            var requests = await _requestAppService.GetRequestsByCustomerIdAsync(customerId.Value, cancellationToken);
            var proposals = await _customerAppService.GetProposalsByCustomerIdAsync(customerId.Value, cancellationToken);

            ViewBag.Orders = orders;
            ViewBag.Requests = requests;
            ViewBag.Proposals = proposals;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            _logger.Information("Dashboard loaded successfully for CustomerId: {CustomerId}", customerId.Value);
            return View(customerDto);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var editDto = await _customerAppService.GetEditCustomerProfileAsync(customerId.Value, cancellationToken);
            if (editDto == null)
            {
                _logger.Warning("Customer not found for CustomerId: {CustomerId}", customerId.Value);
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Provinces = await _locationAppService.GetAllProvincesAsync(cancellationToken);
            ViewBag.Cities = string.IsNullOrEmpty(editDto.State)
                ? new List<CityDto>()
                : await _locationAppService.GetCitiesByProvinceNameAsync(editDto.State, cancellationToken);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(editDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditCustomerDto model, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var customerDto = await _customerAppService.GetByCustomerIdAsync(customerId.Value, cancellationToken);
            if (customerDto == null)
            {
                _logger.Warning("Customer not found for CustomerId: {CustomerId}", customerId.Value);
                return RedirectToAction("Login", "Account");
            }

            model.AppUserId = customerDto.AppUserId;
            _logger.Information("Received model for update: FirstName={FirstName}, LastName={LastName}, PhoneNumber={PhoneNumber}, ProfilePicture={ProfilePicture}",
                model.FirstName, model.LastName, model.PhoneNumber, model.ProfilePicture);

            if (model.ProfilePictureFile == null || model.ProfilePictureFile.Length == 0)
            {
                ModelState.Remove("ProfilePictureFile");
            }

            if (!ModelState.IsValid)
            {
                _logger.Warning("ModelState is invalid for CustomerId: {CustomerId}", customerId.Value);
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.Warning("Validation error: {Error}", error.ErrorMessage);
                }

                ViewBag.Provinces = await _locationAppService.GetAllProvincesAsync(cancellationToken);
                ViewBag.Cities = string.IsNullOrEmpty(model.State)
                    ? new List<CityDto>()
                    : await _locationAppService.GetCitiesByProvinceNameAsync(model.State, cancellationToken);

                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
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
                    _logger.Information("New profile picture uploaded: {ProfilePicture}", model.ProfilePicture);
                }
                else
                {
                    model.ProfilePicture = customerDto.ProfilePicture ?? "default.png";
                    _logger.Information("No new profile picture, keeping existing: {ProfilePicture}", model.ProfilePicture);
                }

                var result = await _customerAppService.UpdateCustomerProfileAsync(model, cancellationToken);
                if (result)
                {
                    _logger.Information("Profile updated successfully for CustomerId: {CustomerId}", customerId.Value);
                    TempData["SuccessMessage"] = "پروفایل شما با موفقیت به‌روزرسانی شد!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    _logger.Warning("Failed to update profile for CustomerId: {CustomerId}", customerId.Value);
                    TempData["ErrorMessage"] = "خطا در به‌روزرسانی پروفایل. لطفاً دوباره تلاش کنید.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in EditProfile for CustomerId: {CustomerId}", customerId.Value);
                TempData["ErrorMessage"] = "خطای سیستمی در به‌روزرسانی پروفایل.";
            }

            ViewBag.Provinces = await _locationAppService.GetAllProvincesAsync(cancellationToken);
            ViewBag.Cities = string.IsNullOrEmpty(model.State)
                ? new List<CityDto>()
                : await _locationAppService.GetCitiesByProvinceNameAsync(model.State, cancellationToken);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(model);
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
                    provinceName, cities?.Count ?? 0, cities);
                return Json(cities);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching cities for Province: {ProvinceName}", provinceName);
                return Json(new List<CityDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Orders(CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _customerAppService.GetOrdersByCustomerIdAsync(customerId.Value, cancellationToken);
            _logger.Information("Orders loaded successfully for CustomerId: {CustomerId}, Count: {OrderCount}", customerId.Value, orders.Count);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Requests(CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var requests = await _requestAppService.GetRequestsByCustomerIdAsync(customerId.Value, cancellationToken);
            _logger.Information("Requests loaded successfully for CustomerId: {CustomerId}, Count: {RequestCount}", customerId.Value, requests?.Count ?? 0);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(requests);
        }

        [HttpGet]
        public async Task<IActionResult> Proposals(CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var proposals = await _customerAppService.GetProposalsByCustomerIdAsync(customerId.Value, cancellationToken);
            _logger.Information("Proposals loaded successfully for CustomerId: {CustomerId}, Count: {ProposalCount}", customerId.Value, proposals.Count);

            foreach (var proposal in proposals)
            {
                _logger.Information("Sending to View - Proposal ID: {Id}, ExpertName: {ExpertName}, SubHomeServiceName: {SubHomeServiceName}, OrderDate: {OrderDate}",
                    proposal.Id,
                    proposal.ExpertName,
                    proposal.SubHomeServiceName,
                    proposal.OrderDate == DateTime.MinValue ? "MinValue" : proposal.OrderDate.ToString());
            }

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(proposals);
        }

        //[HttpGet]
        //public async Task<IActionResult> AcceptProposal(int id, CancellationToken cancellationToken)
        //{
        //    var customerId = await GetCustomerIdFromSession(cancellationToken);
        //    if (!customerId.HasValue)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    try
        //    {
        //        var proposalDto = await _customerAppService.GetProposalByIdAsync(id, cancellationToken);
        //        if (proposalDto == null)
        //        {
        //            _logger.Warning("Proposal not found for ProposalId: {ProposalId}", id);
        //            TempData["ErrorMessage"] = "پیشنهاد یافت نشد.";
        //            return RedirectToAction("Proposals");
        //        }

        //        var request = await _customerAppService.GetRequestByIdAsync(proposalDto.RequestId, cancellationToken);
        //        if (request == null || request.CustomerId != customerId.Value)
        //        {
        //            _logger.Warning("Proposal {Id} does not belong to CustomerId: {CustomerId}", id, customerId.Value);
        //            TempData["ErrorMessage"] = "شما دسترسی به این پیشنهاد ندارید.";
        //            return RedirectToAction("Proposals");
        //        }

        //        if (proposalDto.Status == ProposalStatus.Pending)
        //        {
        //            var success = await _customerAppService.UpdateProposalStatusAsync(id, ProposalStatus.Accepted, cancellationToken);
        //            if (!success)
        //            {
        //                _logger.Warning("Failed to accept proposal {Id} for CustomerId: {CustomerId}", id, customerId.Value);
        //                TempData["ErrorMessage"] = "خطا در تأیید پیشنهاد.";
        //                return RedirectToAction("Proposals");
        //            }
        //            _logger.Information("Proposal {Id} accepted successfully for CustomerId: {CustomerId}", id, customerId.Value);
        //        }

        //        var orderId = await _customerAppService.SelectProposalAndCreateOrderAsync(id, customerId.Value, cancellationToken);
        //        _logger.Information("Order {OrderId} created successfully for ProposalId: {ProposalId}", orderId, id);
        //        TempData["SuccessMessage"] = "پیشنهاد تأیید و سفارش با موفقیت ایجاد شد!";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Failed to process AcceptProposal for ProposalId: {ProposalId}", id);
        //        TempData["ErrorMessage"] = "خطایی در تأیید پیشنهاد یا ایجاد سفارش رخ داد.";
        //    }

        //    return RedirectToAction("Proposals");
        //}

        //[HttpGet]
        //public async Task<IActionResult> RejectProposal(int id, CancellationToken cancellationToken)
        //{
        //    var customerId = await GetCustomerIdFromSession(cancellationToken);
        //    if (!customerId.HasValue)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    var result = await _customerAppService.UpdateProposalStatusAsync(id, ProposalStatus.Rejected, cancellationToken);
        //    if (result)
        //    {
        //        _logger.Information("Proposal {Id} rejected successfully for CustomerId: {CustomerId}", id, customerId.Value);
        //        TempData["SuccessMessage"] = "پیشنهاد با موفقیت رد شد!";
        //    }
        //    else
        //    {
        //        _logger.Warning("Failed to reject proposal {Id} for CustomerId: {CustomerId}", id, customerId.Value);
        //        TempData["ErrorMessage"] = "خطا در رد پیشنهاد.";
        //    }
        //    return RedirectToAction("Proposals");
        //}

        [HttpPost]
        [Route("SelectProposal/{id}")]
        public async Task<IActionResult> SelectProposal(int id, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var proposalDto = await _customerAppService.GetProposalByIdAsync(id, cancellationToken);
                if (proposalDto == null)
                {
                    _logger.Warning("Proposal not found for ProposalId: {ProposalId}", id);
                    TempData["ErrorMessage"] = "پیشنهاد یافت نشد.";
                    return RedirectToAction("Proposals");
                }

                var request = await _customerAppService.GetRequestByIdAsync(proposalDto.RequestId, cancellationToken);
                if (request == null || request.CustomerId != customerId.Value)
                {
                    _logger.Warning("Proposal {Id} does not belong to CustomerId: {CustomerId}", id, customerId.Value);
                    TempData["ErrorMessage"] = "شما دسترسی به این پیشنهاد ندارید.";
                    return RedirectToAction("Proposals");
                }

                if (proposalDto.Status == ProposalStatus.Pending)
                {
                    var success = await _customerAppService.UpdateProposalStatusAsync(id, ProposalStatus.Accepted, cancellationToken);
                    if (!success)
                    {
                        _logger.Warning("Failed to accept proposal with Id: {ProposalId}", id);
                        TempData["ErrorMessage"] = "تغییر وضعیت پیشنهاد با خطا مواجه شد.";
                        return RedirectToAction("Proposals");
                    }
                }
                else if (proposalDto.Status != ProposalStatus.Accepted)
                {
                    _logger.Warning("Proposal {Id} is not in a valid state for selection", id);
                    TempData["ErrorMessage"] = "این پیشنهاد قابل انتخاب نیست.";
                    return RedirectToAction("Proposals");
                }

                var orderId = await _customerAppService.SelectProposalAndCreateOrderAsync(id, customerId.Value, cancellationToken);
                TempData["SuccessMessage"] = "سفارش با موفقیت از پیشنهاد ایجاد شد!";
                return RedirectToAction("Proposals");
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "شما دسترسی به این پیشنهاد ندارید.";
                return RedirectToAction("Proposals");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Proposals");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to process SelectProposal for ProposalId: {ProposalId}", id);
                TempData["ErrorMessage"] = "خطایی در ایجاد سفارش رخ داد.";
                return RedirectToAction("Proposals");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                _logger.Warning("No CustomerId in session, redirecting to Login");
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderAppService.GetAsync(orderId, cancellationToken);
            if (order == null)
            {
                _logger.Warning("Order not found for OrderId: {OrderId}", orderId);
                TempData["ErrorMessage"] = "سفارش یافت نشد.";
                return RedirectToAction("Orders");
            }

            if (order.CustomerId != customerId.Value)
            {
                _logger.Warning("Order {OrderId} does not belong to CustomerId: {CustomerId}", orderId, customerId.Value);
                TempData["ErrorMessage"] = "شما دسترسی به این سفارش ندارید.";
                return RedirectToAction("Orders");
            }

            _logger.Information("Order details loaded successfully for OrderId: {OrderId}, CustomerId: {CustomerId}", orderId, customerId.Value);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int orderId, CancellationToken cancellationToken)
        {
            var appUserId = HttpContext.Session.GetInt32("UserId");
            if (!appUserId.HasValue)
            {
                _logger.Warning("No AppUserId in session for CustomerId, redirecting to Login");
                return RedirectToAction("Login", "Account");
            }

            _logger.Information("Retrieved AppUserId from Session: {AppUserId}", appUserId.Value);

            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var customerUser = await _userAppService.GetByIdAsync(appUserId.Value, cancellationToken);
            if (customerUser == null)
            {
                _logger.Warning("CustomerUser not found for AppUserId: {AppUserId}", appUserId.Value);
                return RedirectToAction("Orders");
            }

            _logger.Information("Fetched CustomerUser: AppUserId: {Id}, Name: {Name}, Role: {Role}",
                customerUser.Id, $"{customerUser.FirstName} {customerUser.LastName}", customerUser.Role);

            var order = await _orderAppService.GetAsync(orderId, cancellationToken);
            if (order == null)
            {
                _logger.Warning("Order {OrderId} not found", orderId);
                return RedirectToAction("Orders");
            }

            _logger.Information("Order loaded: OrderId: {OrderId}, CustomerId: {CustomerId}, ExpertId: {ExpertId}", order.Id, order.CustomerId, order.ExpertId);

            if (order.CustomerId != customerId.Value)
            {
                _logger.Warning("Order {OrderId} does not belong to CustomerId: {CustomerId}", orderId, customerId.Value);
                return RedirectToAction("Orders");
            }

            var expert = await _expertAppService.GetExpertUserByExpertIdAsync(order.ExpertId, cancellationToken);
            if (expert == null)
            {
                _logger.Warning("Expert user not found for ExpertId: {ExpertId}", order.ExpertId);
                return RedirectToAction("Orders");
            }

            _logger.Information("Fetched Expert: ExpertId: {ExpertId}, Name: {Name}, Role: {Role}",
                order.ExpertId, $"{expert.FirstName} {expert.LastName}", expert.Role);

            var model = new PaymentViewModel
            {
                OrderId = orderId,
                CurrentBalance = customerUser.AccountBalance,
                AmountToPay = order.FinalPrice,
                ExpertName = expert != null ? $"{expert.FirstName} {expert.LastName}" : "کارشناس نامشخص"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, CancellationToken cancellationToken)
        {
            _logger.Information("Starting ProcessPayment POST for OrderId: {OrderId}", orderId);

            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                _logger.Warning("No CustomerId in session for OrderId: {OrderId}", orderId);
                TempData["ErrorMessage"] = "لطفاً دوباره وارد شوید.";
                return RedirectToAction("Login", "Account");
            }

            _logger.Information("Processing payment with CustomerId: {CustomerId} for OrderId: {OrderId}", customerId.Value, orderId);

            var success = await _transactionAppService.ProcessPaymentAsync(orderId, customerId.Value, cancellationToken);
            if (success)
            {
                _logger.Information("Payment processed successfully for OrderId: {OrderId}", orderId);
                TempData["SuccessMessage"] = "پرداخت با موفقیت انجام شد!";
            }
            else
            {
                _logger.Warning("Payment failed for OrderId: {OrderId}, CustomerId: {CustomerId}", orderId, customerId.Value);
                TempData["ErrorMessage"] = "پرداخت با مشکل مواجه شد. لطفاً دوباره تلاش کنید.";
            }

            return RedirectToAction("Orders");
        }

        [HttpGet]
        public async Task<IActionResult> CreateReview(int orderId, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = await _reviewAppService.PrepareReviewAsync(orderId, customerId.Value, cancellationToken);
            if (model == null)
            {
                return RedirectToAction("Orders");
            }

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDto model, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                return View(model);
            }

            var success = await _reviewAppService.CreateAsync(model, customerId.Value, cancellationToken);
            if (success)
            {
                TempData["SuccessMessage"] = "نظر شما با موفقیت ثبت شد!";
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در ثبت نظر.";
            }

            return RedirectToAction("Orders");
        }

        [HttpGet]
        public async Task<IActionResult> SubHomeServicesList(int homeServiceId, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var homeService = await _homeServiceAppService.GetAsync(homeServiceId, cancellationToken);
                if (homeService == null)
                {
                    _logger.Warning("HomeService not found for ID: {HomeServiceId}", homeServiceId);
                    TempData["ErrorMessage"] = "سرویس مورد نظر یافت نشد.";
                    return RedirectToAction("ServiceHierarchy");
                }

                var subHomeServices = await _subHomeServiceAppService.GetSubHomeServicesByHomeServiceIdAsync(homeServiceId, cancellationToken);

                ViewBag.HomeServiceName = homeService.Name;
                ViewBag.HomeServiceId = homeServiceId;
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

                _logger.Information("Loaded {Count} sub-home services for HomeServiceId: {HomeServiceId}",
                    subHomeServices?.Count ?? 0, homeServiceId);

                if (subHomeServices != null && subHomeServices.Any())
                {
                    foreach (var subService in subHomeServices)
                    {
                        await _subHomeServiceAppService.IncrementViewCountAsync(subService.Id, cancellationToken);
                    }
                }

                return View(subHomeServices);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading sub-home services for HomeServiceId: {HomeServiceId}", homeServiceId);
                TempData["ErrorMessage"] = "خطا در بارگذاری زیر سرویس‌ها.";
                return RedirectToAction("ServiceHierarchy");
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyOrders(CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CustomerOrdersViewModel();

            model.Requests = await _requestAppService.GetRequestsByCustomerIdAsync(customerId.Value, cancellationToken);

            model.Proposals = await _customerAppService.GetProposalsByCustomerIdAsync(customerId.Value, cancellationToken);

            var orders = await _orderAppService.GetByCustomerIdAsync(customerId.Value, cancellationToken);

            if (orders != null && orders.Any())
            {
                model.ActiveOrders = orders.Where(o => o.Status != RequestStatus.Completed && o.IsActive).ToList();
                model.CompletedOrders = orders.Where(o => o.Status == RequestStatus.Completed && o.IsActive).ToList();
            }

            model.Reviews = await _reviewAppService.GetByCustomerIdAsync(customerId.Value, cancellationToken);

            _logger.Information("MyOrders loaded successfully for CustomerId: {CustomerId}. Requests: {RequestCount}, Proposals: {ProposalCount}, ActiveOrders: {ActiveOrderCount}, CompletedOrders: {CompletedOrderCount}",
                customerId.Value, model.Requests.Count, model.Proposals.Count, model.ActiveOrders.Count, model.CompletedOrders.Count);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProposalsList(int requestId, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _requestAppService.GetAsync(requestId, cancellationToken);
            if (request == null || request.CustomerId != customerId.Value)
            {
                _logger.Warning("Request not found or doesn't belong to CustomerId: {CustomerId}", customerId.Value);
                TempData["ErrorMessage"] = "درخواست مورد نظر یافت نشد یا متعلق به شما نیست.";
                return RedirectToAction("MyOrders");
            }

            var proposals = await _proposalAppService.GetProposalsByRequestIdAsync(requestId, cancellationToken);

            var orders = await _orderAppService.GetByCustomerIdAsync(customerId.Value, cancellationToken);
            var hasActiveOrder = orders != null && orders.Any(o => o.RequestId == requestId && o.IsActive);

            var model = new ProposalsListViewModel
            {
                Request = request,
                Proposals = proposals ?? new List<ProposalDto>(),
                HasActiveOrder = hasActiveOrder
            };

            _logger.Information("ProposalsList loaded successfully for RequestId: {RequestId}, CustomerId: {CustomerId}. Proposals: {ProposalCount}, HasActiveOrder: {HasActiveOrder}",
                requestId, customerId.Value, model.Proposals.Count, model.HasActiveOrder);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AcceptProposal(int id, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var proposal = await _customerAppService.GetProposalByIdAsync(id, cancellationToken);
                if (proposal == null)
                {
                    _logger.Warning("Proposal not found with ID: {ProposalId}", id);
                    TempData["ErrorMessage"] = "پیشنهاد مورد نظر یافت نشد.";
                    return RedirectToAction("MyOrders");
                }

                var request = await _customerAppService.GetRequestByIdAsync(proposal.RequestId, cancellationToken);
                if (request.CustomerId != customerId.Value)
                {
                    _logger.Warning("Proposal {Id} does not belong to CustomerId: {CustomerId}", id, customerId.Value);
                    TempData["ErrorMessage"] = "شما دسترسی به این پیشنهاد ندارید.";
                    return RedirectToAction("MyOrders");
                }

                if (proposal.Status != ProposalStatus.Pending)
                {
                    _logger.Warning("Cannot accept proposal {Id} with status: {Status}", id, proposal.Status);
                    TempData["ErrorMessage"] = "این پیشنهاد قابل پذیرش نیست.";
                    return RedirectToAction("ProposalsList", new { requestId = proposal.RequestId });
                }

                var statusUpdated = await _customerAppService.UpdateProposalStatusAsync(id, ProposalStatus.Accepted, cancellationToken);
                if (!statusUpdated)
                {
                    _logger.Warning("Failed to update status for proposal {Id}", id);
                    TempData["ErrorMessage"] = "خطا در به‌روزرسانی وضعیت پیشنهاد.";
                    return RedirectToAction("ProposalsList", new { requestId = proposal.RequestId });
                }

                var orderId = await _customerAppService.SelectProposalAndCreateOrderAsync(id, customerId.Value, cancellationToken);

                var updateRequestDto = new UpdateRequestDto
                {
                    Status = RequestStatus.InProgress,
                    Deadline = request.Deadline,
                    ExecutionDate = proposal.ExecutionDate
                };
                await _requestAppService.UpdateAsync(request.Id, updateRequestDto, cancellationToken);

                _logger.Information("Proposal {Id} accepted and order {OrderId} created successfully for CustomerId: {CustomerId}",
                    id, orderId, customerId.Value);
                TempData["SuccessMessage"] = "پیشنهاد با موفقیت پذیرفته و سفارش ایجاد شد.";

                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error accepting proposal {Id} for CustomerId: {CustomerId}", id, customerId.Value);
                TempData["ErrorMessage"] = "خطا در پذیرش پیشنهاد.";
                return RedirectToAction("MyOrders");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RejectProposal(int id, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var proposal = await _customerAppService.GetProposalByIdAsync(id, cancellationToken);
                if (proposal == null)
                {
                    _logger.Warning("Proposal not found with ID: {ProposalId}", id);
                    TempData["ErrorMessage"] = "پیشنهاد مورد نظر یافت نشد.";
                    return RedirectToAction("MyOrders");
                }

                var request = await _customerAppService.GetRequestByIdAsync(proposal.RequestId, cancellationToken);
                if (request.CustomerId != customerId.Value)
                {
                    _logger.Warning("Proposal {Id} does not belong to CustomerId: {CustomerId}", id, customerId.Value);
                    TempData["ErrorMessage"] = "شما دسترسی به این پیشنهاد ندارید.";
                    return RedirectToAction("MyOrders");
                }

                if (proposal.Status != ProposalStatus.Pending)
                {
                    _logger.Warning("Cannot reject proposal {Id} with status: {Status}", id, proposal.Status);
                    TempData["ErrorMessage"] = "این پیشنهاد قابل رد کردن نیست.";
                    return RedirectToAction("ProposalsList", new { requestId = proposal.RequestId });
                }

                var result = await _customerAppService.UpdateProposalStatusAsync(id, ProposalStatus.Rejected, cancellationToken);

                if (result)
                {
                    _logger.Information("Proposal {Id} rejected successfully for CustomerId: {CustomerId}", id, customerId.Value);
                    TempData["SuccessMessage"] = "پیشنهاد با موفقیت رد شد.";
                }
                else
                {
                    _logger.Warning("Failed to reject proposal {Id} for CustomerId: {CustomerId}", id, customerId.Value);
                    TempData["ErrorMessage"] = "خطا در رد پیشنهاد.";
                }

                return RedirectToAction("ProposalsList", new { requestId = proposal.RequestId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error rejecting proposal {Id} for CustomerId: {CustomerId}", id, customerId.Value);
                TempData["ErrorMessage"] = "خطا در رد پیشنهاد.";
                return RedirectToAction("MyOrders");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CancelRequest(int id, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdFromSession(cancellationToken);
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _requestAppService.GetAsync(id, cancellationToken);
            if (request == null || request.CustomerId != customerId.Value)
            {
                _logger.Warning("Request not found or doesn't belong to CustomerId: {CustomerId}", customerId.Value);
                TempData["ErrorMessage"] = "درخواست مورد نظر یافت نشد یا متعلق به شما نیست.";
                return RedirectToAction("MyOrders");
            }

            var orders = await _orderAppService.GetByCustomerIdAsync(customerId.Value, cancellationToken);
            var hasActiveOrder = orders != null && orders.Any(o => o.RequestId == id && o.IsActive);

            if (hasActiveOrder)
            {
                _logger.Warning("Cannot cancel request {Id} as it has active orders", id);
                TempData["ErrorMessage"] = "این درخواست به دلیل داشتن سفارش فعال قابل لغو نیست.";
                return RedirectToAction("MyOrders");
            }

            var updateRequestDto = new UpdateRequestDto
            {
                Status = RequestStatus.Cancelled,
                Deadline = request.Deadline,
                ExecutionDate = request.ExecutionDate,
                EnvironmentImagePath = request.EnvironmentImagePath
            };

            var result = await _requestAppService.UpdateAsync(id, updateRequestDto, cancellationToken);

            if (result)
            {
                _logger.Information("Request {Id} cancelled successfully for CustomerId: {CustomerId}", id, customerId.Value);
                TempData["SuccessMessage"] = "درخواست با موفقیت لغو شد.";
            }
            else
            {
                _logger.Warning("Failed to cancel request {Id} for CustomerId: {CustomerId}", id, customerId.Value);
                TempData["ErrorMessage"] = "خطا در لغو درخواست.";
            }

            return RedirectToAction("MyOrders");
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

