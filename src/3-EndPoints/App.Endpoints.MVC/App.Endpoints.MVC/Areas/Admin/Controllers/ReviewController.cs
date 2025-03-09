using App.Domain.Core.Services.Interfaces.IAppService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly IReviewAppService _reviewAppService;
        private readonly Serilog.ILogger _logger;

        public ReviewController(IReviewAppService reviewAppService, Serilog.ILogger logger)
        {
            _reviewAppService = reviewAppService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            _logger.Information("Admin: Fetching all reviews.");
            var reviews = await _reviewAppService.GetAllAsync(cancellationToken);
            return View(reviews);
        }

        public async Task<IActionResult> ByOrder(int orderId, CancellationToken cancellationToken)
        {
            _logger.Information("Admin: Fetching reviews for order ID: {OrderId}", orderId);
            var reviews = await _reviewAppService.GetByOrderIdAsync(orderId, cancellationToken);
            return View("Index", reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Approve button clicked for review ID: {Id}", id);
            var result = await _reviewAppService.ApproveAsync(id, cancellationToken);
            if (result)
            {
                _logger.Information("Review ID: {Id} approved successfully", id);
                TempData["SuccessMessage"] = "کامنت تأیید شد!";
            }
            else
            {
                _logger.Warning("Failed to approve review ID: {Id}", id);
                TempData["ErrorMessage"] = "خطا در تأیید کامنت.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Reject called for review ID: {Id}", id);
            var result = await _reviewAppService.RejectAsync(id, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "کامنت رد شد!";
                _logger.Information("Review ID: {Id} rejected successfully.", id);
            }
            else
            {
                TempData["ErrorMessage"] = "خطا در رد کامنت.";
                _logger.Warning("Failed to reject review ID: {Id}", id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
