using App.Domain.Core.DTO.Categories;
using App.Domain.Core.Services.Interfaces.IAppService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryAppService _categoryAppService;
        private readonly Serilog.ILogger _logger;

        public CategoryController(ICategoryAppService categoryAppService, Serilog.ILogger logger)
        {
            _categoryAppService = categoryAppService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all categories for Index");
            var categories = await _categoryAppService.GetAllAsync(cancellationToken);
            _logger.Information("Fetched {Count} categories", categories.Count);
            return View(categories);
        }

        public IActionResult Create()
        {
            return View(new CreateCategoryDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto, IFormFile imageFile, CancellationToken cancellationToken)
        {
            _logger.Information("Received Create POST request for Category with Name: {Name}", dto.Name);

            if (dto == null)
            {
                TempData["ErrorMessage"] = "داده‌های فرم دریافت نشد.";
                _logger.Warning("DTO is null for Category creation");
                return View(new CreateCategoryDto());
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                dto.ImageFile = imageFile;
                var directory = Path.Combine("wwwroot", "images", "Categories");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                var fileName = Path.GetFileName(imageFile.FileName);
                var imagePath = Path.Combine(directory, fileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                dto.ImagePath = $"/images/Categories/{fileName}";
                _logger.Information("Image uploaded to: {ImagePath}", dto.ImagePath);
            }
            else
            {
                dto.ImagePath = "/images/Categories/default.jpg";
                _logger.Information("No image uploaded, using default ImagePath: {ImagePath}", dto.ImagePath);
            }

            ModelState.Clear();

            try
            {
                var result = await _categoryAppService.CreateAsync(dto, cancellationToken);
                if (result)
                {
                    TempData["SuccessMessage"] = "دسته‌بندی با موفقیت اضافه شد.";
                    _logger.Information("Category created successfully with Name: {Name}", dto.Name);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "خطا در اضافه کردن دسته‌بندی.";
                    _logger.Warning("Failed to create Category with Name: {Name}", dto.Name);
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "خطای غیرمنتظره: " + ex.Message;
                _logger.Error(ex, "Unexpected error while creating Category with Name: {Name}", dto.Name);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching Category for edit with ID: {Id}", id);
            var category = await _categoryAppService.GetAsync(id, cancellationToken);
            if (category == null)
            {
                _logger.Warning("Category with ID: {Id} not found.", id);
                TempData["ErrorMessage"] = "دسته‌بندی پیدا نشد.";
                return RedirectToAction("Index");
            }
            var updateDto = new UpdateCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImagePath = category.ImagePath,
                IsActive = category.IsActive
            };
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto dto, IFormFile imageFile, CancellationToken cancellationToken)
        {
            _logger.Information("Received Edit POST request for Category with ID: {Id}, Name={Name}, IsActive={IsActive}, ImagePath={ImagePath}",
                id, dto.Name, dto.IsActive, dto.ImagePath);

            ModelState.Clear();

            if (imageFile != null && imageFile.Length > 0)
            {
                var directory = Path.Combine("wwwroot", "images", "Categories");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                var fileName = Path.GetFileName(imageFile.FileName);
                var imagePath = Path.Combine(directory, fileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                dto.ImagePath = $"/images/Categories/{fileName}";
                _logger.Information("Image uploaded to: {ImagePath}", dto.ImagePath);
            }
            else
            {
                _logger.Information("No new image uploaded, keeping existing ImagePath: {ImagePath}", dto.ImagePath);
            }

            var result = await _categoryAppService.UpdateAsync(id, dto, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "دسته‌بندی با موفقیت ویرایش شد.";
                _logger.Information("Category with ID: {Id} updated successfully", id);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در ویرایش دسته‌بندی.";
                _logger.Warning("Failed to update Category with ID: {Id}", id);
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching Category for delete with ID: {Id}", id);
            var category = await _categoryAppService.GetAsync(id, cancellationToken);
            if (category == null)
            {
                _logger.Warning("Category with ID: {Id} not found.", id);
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to deactivate Category with ID: {Id}", id);
            var result = await _categoryAppService.DeleteAsync(id, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "دسته‌بندی با موفقیت غیرفعال شد.";
                _logger.Information("Category with ID: {Id} deactivated successfully", id);
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در غیرفعال کردن دسته‌بندی.";
                _logger.Warning("Failed to deactivate Category with ID: {Id}", id);
            }
            return RedirectToAction("Index");
        }
    }
}
