using App.Domain.Core.DTO.Reviews;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IService;
using HomeService.Domain.AppServices.OrderAppServices;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.AppServices.ReviewAppServices
{
    public class ReviewAppService : IReviewAppService
    {
        private readonly IReviewService _reviewService;
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger _logger;

        public ReviewAppService(IReviewService reviewService, IOrderAppService orderAppService, ILogger logger)
        {
            _reviewService = reviewService;
            _orderAppService = orderAppService;
            _logger = logger;
        }

        public Task<List<ReviewDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _reviewService.GetAllAsync(cancellationToken);
        }

        public Task<List<ReviewDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            return _reviewService.GetByOrderIdAsync(orderId, cancellationToken);
        }

        public async Task<CreateReviewDto> PrepareReviewAsync(int orderId, int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Preparing review form for OrderId: {OrderId}, CustomerId: {CustomerId}", orderId, customerId);

            var order = await _orderAppService.GetAsync(orderId, cancellationToken);
            if (order == null || order.CustomerId != customerId || order.PaymentStatus != PaymentStatus.Completed)
            {
                _logger.Warning("Order {OrderId} not found, not completed, or does not belong to CustomerId: {CustomerId}", orderId, customerId);
                return null;
            }

            var existingReviews = await _reviewService.GetByOrderIdAsync(orderId, cancellationToken);
            if (existingReviews.Any())
            {
                _logger.Warning("Review already exists for OrderId: {OrderId}", orderId);
                return null;
            }

            return new CreateReviewDto
            {
                CustomerId = customerId,
                ExpertId = order.ExpertId,
                OrderId = orderId,
                Rating = 5
            };
        }

        public async Task<bool> CreateAsync(CreateReviewDto dto, int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Creating review for OrderId: {OrderId}, CustomerId: {CustomerId}", dto.OrderId, customerId);

            var order = await _orderAppService.GetAsync(dto.OrderId, cancellationToken);
            if (order == null || order.CustomerId != customerId || order.PaymentStatus != PaymentStatus.Completed)
            {
                _logger.Warning("Order {OrderId} not found, not completed, or does not belong to CustomerId: {CustomerId}", dto.OrderId, customerId);
                return false;
            }

            var existingReviews = await _reviewService.GetByOrderIdAsync(dto.OrderId, cancellationToken);
            if (existingReviews.Any())
            {
                _logger.Warning("Review already exists for OrderId: {OrderId}", dto.OrderId);
                return false;
            }

            dto.CustomerId = customerId;
            dto.ExpertId = order.ExpertId;

            return await _reviewService.CreateAsync(dto, cancellationToken);
        }

        public Task<bool> ApproveAsync(int id, CancellationToken cancellationToken)
        {
            return _reviewService.ApproveAsync(id, cancellationToken);
        }

        public Task<bool> RejectAsync(int id, CancellationToken cancellationToken)
        {
            return _reviewService.RejectAsync(id, cancellationToken);
        }
        public async Task<List<ReviewDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching reviews for CustomerId: {CustomerId}", customerId);
            return await _reviewService.GetByCustomerIdAsync(customerId, cancellationToken);
        }

    }

}
