using App.Domain.Core.DTO.Orders;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Interfaces.IAppService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderAppService _orderAppService;
        private readonly Serilog.ILogger _logger;

        public OrderController(IOrderAppService orderAppService, Serilog.ILogger logger)
        {
            _orderAppService = orderAppService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            _logger.Information("Loading all orders for admin panel.");
            var orders = await _orderAppService.GetAllAsync(cancellationToken);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var orderDto = await _orderAppService.GetOrderForEditAsync(id, cancellationToken);
            if (orderDto == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(orderDto);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateOrderDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _orderAppService.UpdateAsync(id, dto, cancellationToken);
            if (!result)
            {
                TempData["ErrorMessage"] = "خطا! سفارش مورد نظر یافت نشد.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "سفارش با موفقیت بروزرسانی شد.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(int id, PaymentStatus status, CancellationToken cancellationToken)
        {
            var result = await _orderAppService.UpdatePaymentStatusAsync(id, status, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "Payment status updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update payment status.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var order = await _orderAppService.GetAsync(id, cancellationToken);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var result = await _orderAppService.DeleteAsync(id, cancellationToken);
            if (result)
            {
                TempData["SuccessMessage"] = "Order deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete order.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
