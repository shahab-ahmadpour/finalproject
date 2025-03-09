using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.Services.Interfaces.IAppService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Endpoints.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeServiceController : Controller
    {
        private readonly IHomeServiceAppService _homeServiceAppService;
        private readonly Serilog.ILogger _logger;

        public HomeServiceController(IHomeServiceAppService homeServiceAppService, Serilog.ILogger logger)
        {
            _homeServiceAppService = homeServiceAppService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var homeServices = await _homeServiceAppService.GetAllAsync(cancellationToken);
            return View(homeServices);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var categories = await _homeServiceAppService.GetAllCategoriesForDropdownAsync(cancellationToken);
            _logger.Information("Controller: Categories loaded: {Count}", categories?.Count ?? 0);
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            _logger.Information("Controller: ViewBag.Categories set with {Count} items", ((SelectList)ViewBag.Categories).Count());
            return View(new CreateHomeServiceDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHomeServiceDto dto, IFormFile? imageFile, CancellationToken cancellationToken)
        {
            _logger.Information("Received DTO - Name: {Name}, Description: {Description}, CategoryId: {CategoryId}",
                dto.Name, dto.Description, dto.CategoryId);

            ModelState.Clear();

            if (imageFile != null && imageFile.Length > 0)
            {
                var directory = Path.Combine("wwwroot", "images", "HomeServices");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                var imagePath = Path.Combine(directory, imageFile.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                dto.ImagePath = "/images/HomeServices/" + imageFile.FileName;
                _logger.Information("Image uploaded to: {ImagePath}", dto.ImagePath);
            }

            var result = await _homeServiceAppService.CreateAsync(dto, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "سرویس با موفقیت اضافه شد!";
                _logger.Information("HomeService created successfully with Name: {Name}", dto.Name);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در اضافه کردن سرویس.";
                _logger.Warning("Failed to create HomeService with Name: {Name}", dto.Name);
            }

            var categories = await _homeServiceAppService.GetAllCategoriesForDropdownAsync(cancellationToken);
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(dto);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching HomeService for edit with ID: {Id}", id);
            var service = await _homeServiceAppService.GetAsync(id, cancellationToken);
            if (service == null)
            {
                _logger.Warning("Home service with ID: {Id} not found.", id);
                TempData["ErrorMessage"] = "سرویس پیدا نشد.";
                return RedirectToAction("Index");
            }
            var updateDto = new UpdateHomeServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                ImagePath = service.ImagePath,
                CategoryId = service.CategoryId,
                IsActive = service.IsActive
            };

            var categories = await _homeServiceAppService.GetAllCategoriesForDropdownAsync(cancellationToken);
            ViewBag.Categories = new SelectList(categories, "Id", "Name", updateDto.CategoryId);
            _logger.Information("Loaded {Count} categories for dropdown", categories?.Count ?? 0);

            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateHomeServiceDto dto, IFormFile imageFile, CancellationToken cancellationToken)
        {
            _logger.Information("Received Edit POST request for HomeService with ID: {Id}", id);
            _logger.Information("DTO values: Name={Name}, CategoryId={CategoryId}, IsActive={IsActive}, ImagePath={ImagePath}",
                dto.Name, dto.CategoryId, dto.IsActive, dto.ImagePath);

            ModelState.Clear();

            if (imageFile != null && imageFile.Length > 0)
            {
                var directory = Path.Combine("wwwroot", "images", "HomeServices");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                var imagePath = Path.Combine(directory, imageFile.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                dto.ImagePath = "/images/HomeServices/" + imageFile.FileName;
                _logger.Information("Image uploaded to: {ImagePath}", dto.ImagePath);
            }

            var result = await _homeServiceAppService.UpdateAsync(id, dto, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "سرویس با موفقیت به‌روزرسانی شد.";
                _logger.Information("HomeService with ID: {Id} updated successfully", id);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در به‌روزرسانی سرویس.";
                _logger.Warning("Failed to update HomeService with ID: {Id}", id);
            }

            var categories = await _homeServiceAppService.GetAllCategoriesForDropdownAsync(cancellationToken);
            ViewBag.Categories = new SelectList(categories, "Id", "Name", dto.CategoryId);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var service = await _homeServiceAppService.GetAsync(id, cancellationToken);
            if (service == null)
            {
                _logger.Warning("Home service with ID: {Id} not found.", id);
                return NotFound();
            }
            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to deactivate home service with ID: {Id}", id);
            var result = await _homeServiceAppService.DeleteAsync(id, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "سرویس با موفقیت غیرفعال شد.";
                _logger.Information("Home service with ID: {Id} deactivated successfully.", id);
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در غیرفعال کردن سرویس.";
                _logger.Warning("Failed to deactivate home service with ID: {Id}", id);
            }
            return RedirectToAction("Index");
        }
    }
}
