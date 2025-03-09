using App.Domain.Core.DTO.Reviews;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.ReviewServices
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger _logger;

        public ReviewService(IReviewRepository reviewRepository, ILogger logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public Task<List<ReviewDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _reviewRepository.GetAllAsync(cancellationToken);
        }

        public Task<List<ReviewDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            return _reviewRepository.GetByOrderIdAsync(orderId, cancellationToken);
        }

        public Task<bool> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken)
        {
            return _reviewRepository.CreateAsync(dto, cancellationToken);
        }

        public Task<bool> ApproveAsync(int id, CancellationToken cancellationToken)
        {
            return _reviewRepository.ApproveAsync(id, cancellationToken);
        }

        public Task<bool> RejectAsync(int id, CancellationToken cancellationToken)
        {
            return _reviewRepository.RejectAsync(id, cancellationToken);
        }

        public Task<List<Review>> GetAllReviewsAsync(CancellationToken cancellationToken = default)
        {
            return _reviewRepository.GetAllReviewsAsync(cancellationToken);
        }
        public async Task<List<ReviewDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching reviews for CustomerId: {CustomerId}", customerId);
            try
            {
                return await _reviewRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error fetching reviews for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }
    }

}
