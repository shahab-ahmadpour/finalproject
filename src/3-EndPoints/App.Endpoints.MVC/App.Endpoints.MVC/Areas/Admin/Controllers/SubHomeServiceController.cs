using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Interfaces.IAppService;
using HomeService.Domain.AppServices.SubHomeSerAppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SubHomeServiceController : Controller
    {
        private readonly ISubHomeServiceAppService _subHomeServiceAppService;
        private readonly Serilog.ILogger _logger;

        public SubHomeServiceController(ISubHomeServiceAppService subHomeServiceAppService, Serilog.ILogger logger)
        {
            _subHomeServiceAppService = subHomeServiceAppService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var subHomeServices = await _subHomeServiceAppService.GetAllAsync(cancellationToken);
            return View(subHomeServices);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateSubHomeServiceDto();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubHomeServiceDto dto, IFormFile imageFile, CancellationToken cancellationToken)
        {
            if (dto == null)
            {
                TempData["ErrorMessage"] = "داده‌های فرم دریافت نشد.";
                return View(new CreateSubHomeServiceDto());
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                dto.ImageFile = imageFile;
            }

            ModelState.Clear();

            try
            {
                var result = await _subHomeServiceAppService.CreateAsync(dto, cancellationToken);
                if (result)
                {
                    TempData["SuccessMessage"] = "سرویس جدید با موفقیت ساخته شد";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "خطا در ساخت سرویس جدید";
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "خطای غیرمنتظره: " + ex.Message;
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var subHomeServiceDto = await _subHomeServiceAppService.GetSubHomeServiceForEditAsync(id, cancellationToken);

            if (subHomeServiceDto == null)
            {
                TempData["ErrorMessage"] = "سرویس پیدا نشد";
                return RedirectToAction(nameof(Index));
            }

            return View(subHomeServiceDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSubHomeServiceDto dto, IFormFile? imageFile, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    _logger.Information("Uploading new image: {ImageName}", imageFile.FileName);

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "subhome");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var imagePath = Path.Combine(uploadsFolder, imageFile.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    dto.ImagePath = "/images/subhome/" + imageFile.FileName;
                    _logger.Information("ImagePath set to: {ImagePath}", dto.ImagePath);
                }
                else
                {
                    _logger.Warning("ImageFile is null or empty, keeping the existing ImagePath: {ExistingImagePath}", dto.ImagePath);
                }

                var result = await _subHomeServiceAppService.UpdateAsync(id, dto, cancellationToken);
                if (result)
                {
                    TempData["SuccessMessage"] = "سرویس با موفقیت ویرایش شد";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "خطا در ویرایش سرویس";
                }
            }

            _logger.Warning("Edit form has validation errors: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(dto);
        }



        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var subHomeService = await _subHomeServiceAppService.GetAsync(id, cancellationToken);
            if (subHomeService == null)
            {
                TempData["ErrorMessage"] = "سرویس پیدا نشد";
                return RedirectToAction(nameof(Index));
            }

            return View(subHomeService);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var result = await _subHomeServiceAppService.DeleteAsync(id, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "سرویس با موفقیت غیرفعال شد";
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در غیرفعال شدن سرویس";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
