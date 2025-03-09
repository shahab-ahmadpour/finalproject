using App.Domain.Core.DTO.Reviews;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IRepository
{
    public interface IReviewRepository
    {
        Task<List<ReviewDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<ReviewDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken);
        Task<bool> CreateAsync(CreateReviewDto dto, CancellationToken cancellationToken);
        Task<bool> ApproveAsync(int id, CancellationToken cancellationToken);
        Task<bool> RejectAsync(int id, CancellationToken cancellationToken);
        Task<List<Review>> GetAllReviewsAsync(CancellationToken cancellationToken = default);
        Task<List<ReviewDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    }
}
