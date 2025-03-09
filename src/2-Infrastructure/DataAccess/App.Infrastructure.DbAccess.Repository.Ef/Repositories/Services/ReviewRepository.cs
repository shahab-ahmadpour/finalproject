using App.Domain.Core.DTO.Reviews;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Services
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public ReviewRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<ReviewDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all reviews");
            return await _dbContext.Reviews
                .AsNoTracking()
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    CustomerName = r.Customer.AppUser.FirstName + " " + r.Customer.AppUser.LastName,
                    ExpertId = r.ExpertId,
                    ExpertName = r.Expert.AppUser.FirstName + " " + r.Expert.AppUser.LastName,
                    OrderId = r.OrderId,
                    OrderDescription = r.Order.Request.Description,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ReviewDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching reviews for OrderId: {OrderId}", orderId);
            return await _dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.OrderId == orderId)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    CustomerName = r.Customer.AppUser.FirstName + " " + r.Customer.AppUser.LastName,
                    ExpertId = r.ExpertId,
                    ExpertName = r.Expert.AppUser.FirstName + " " + r.Expert.AppUser.LastName,
                    OrderId = r.OrderId,
                    OrderDescription = r.Order.Request.Description,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating review for OrderId: {OrderId}", dto.OrderId);
            var review = new Review
            {
                CustomerId = dto.CustomerId,
                ExpertId = dto.ExpertId,
                OrderId = dto.OrderId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _dbContext.Reviews.Add(review);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Review created successfully for OrderId: {OrderId}", dto.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create review for OrderId: {OrderId}", dto.OrderId);
                return false;
            }
        }

        public async Task<bool> ApproveAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Approving review with Id: {Id}", id);
            var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            if (review == null)
            {
                _logger.Warning("Review not found for Id: {Id}", id);
                return false;
            }

            review.IsApproved = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RejectAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Rejecting review with Id: {Id}", id);
            var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            if (review == null)
            {
                _logger.Warning("Review not found for Id: {Id}", id);
                return false;
            }

            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<Review>> GetAllReviewsAsync(CancellationToken cancellationToken = default)
        {
            _logger.Information("Fetching all raw reviews");
            return await _dbContext.Reviews.AsNoTracking().ToListAsync(cancellationToken);
        }
        public async Task<List<ReviewDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching reviews for CustomerId: {CustomerId}", customerId);
            try
            {
                var reviews = await _dbContext.Reviews
                    .Where(r => r.CustomerId == customerId)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        OrderId = r.OrderId,
                        CustomerId = r.CustomerId,
                        ExpertId = r.ExpertId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        IsApproved = r.IsApproved,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync(cancellationToken);

                if (reviews == null || !reviews.Any())
                {
                    _logger.Warning("No reviews found for CustomerId: {CustomerId}", customerId);
                    return new List<ReviewDto>();
                }

                return reviews;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching reviews for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }
    }
}
