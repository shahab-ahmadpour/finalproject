using App.Domain.Core.DTO.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IAppService
{
    public interface IReviewAppService
    {
        Task<List<ReviewDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<ReviewDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken);
        Task<CreateReviewDto> PrepareReviewAsync(int orderId, int customerId, CancellationToken cancellationToken);
        Task<bool> CreateAsync(CreateReviewDto dto, int customerId, CancellationToken cancellationToken);
        Task<bool> ApproveAsync(int id, CancellationToken cancellationToken);
        Task<bool> RejectAsync(int id, CancellationToken cancellationToken);
        Task<List<ReviewDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    }

}
