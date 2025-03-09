using App.Domain.Core.DTO.Orders;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Interfaces.IAppService
{
    public interface IOrderAppService
    {
        Task<bool> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateOrderDto dto, CancellationToken cancellationToken);
        Task<OrderDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatus status, CancellationToken cancellationToken);
        Task<UpdateOrderDto> GetOrderForEditAsync(int id, CancellationToken cancellationToken);
        Task<List<OrderDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
        Task<bool> CreateOrderFromProposalAsync(int proposalId, int customerId, CancellationToken cancellationToken);
        Task<int> ProcessProposalSelectionAsync(int proposalId, int customerId, CancellationToken cancellationToken);
        Task<Order> GetByProposalIdAsync(int proposalId, CancellationToken cancellationToken);

        Task<List<OrderDto>> GetOrdersByExpertIdAsync(int expertId, CancellationToken cancellationToken);
    }
}