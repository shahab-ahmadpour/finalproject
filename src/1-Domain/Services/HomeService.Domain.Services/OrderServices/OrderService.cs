using App.Domain.Core.DTO.Orders;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;

        public OrderService(IOrderRepository orderRepository, ILogger logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating new order with ProposalId: {ProposalId}");
            return await _orderRepository.CreateAsync(dto, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, UpdateOrderDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating order with Id: {Id}");
            return await _orderRepository.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<OrderDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Getting order with Id: {Id}");
            return await _orderRepository.GetAsync(id, cancellationToken);
        }

        public async Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Getting all orders");
            return await _orderRepository.GetAllAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting order with Id: {Id}");
            return await _orderRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatus status, CancellationToken cancellationToken)
        {
            _logger.Information("Updating payment status for order Id: {Id} to {Status}", id, status);
            return await _orderRepository.UpdatePaymentStatusAsync(id, status, cancellationToken);
        }

        public async Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Getting all orders (entities)");
            return await _orderRepository.GetAllOrdersAsync(cancellationToken);
        }

        public async Task<List<OrderDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Getting orders for CustomerId: {CustomerId}", customerId);

            try
            {
                var orders = await _orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
                if (orders == null || !orders.Any())
                {
                    _logger.Warning("No orders found for CustomerId: {CustomerId}", customerId);
                    return new List<OrderDto>();
                }

                var result = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer?.AppUser?.FirstName + " " + o.Customer?.AppUser?.LastName ?? "نامشخص",
                    ExpertId = o.ExpertId,
                    ExpertName = o.Expert?.AppUser?.FirstName + " " + o.Expert?.AppUser?.LastName ?? "نامشخص",
                    RequestId = o.RequestId,
                    RequestDescription = o.Request?.Description ?? "بدون توضیح",
                    SubHomeServiceName = o.Request?.SubHomeService?.Name ?? "نامشخص",
                    FinalPrice = o.FinalPrice,
                    PaymentStatus = o.PaymentStatus,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    OrderDate = o.CreatedAt
                }).ToList();

                _logger.Information("Found {Count} orders for CustomerId: {CustomerId}", result.Count, customerId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting orders for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<Order> GetByProposalIdAsync(int proposalId, CancellationToken cancellationToken)
        {
            _logger.Information("Getting order by ProposalId: {ProposalId}", proposalId);
            return await _orderRepository.GetByProposalIdAsync(proposalId, cancellationToken);
        }

        public async Task<List<OrderDto>> GetOrdersByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Getting orders for ExpertId: {ExpertId}", expertId);

            try
            {
                var orders = await _orderRepository.GetByExpertIdAsync(expertId, cancellationToken);
                if (orders == null || !orders.Any())
                {
                    _logger.Warning("Service: No orders found for ExpertId: {ExpertId}", expertId);
                    return new List<OrderDto>();
                }

                var result = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer?.AppUser?.FirstName + " " + o.Customer?.AppUser?.LastName ?? "نامشخص",
                    ExpertId = o.ExpertId,
                    ExpertName = o.Expert?.AppUser?.FirstName + " " + o.Expert?.AppUser?.LastName ?? "نامشخص",
                    RequestId = o.RequestId,
                    RequestDescription = o.Request?.Description ?? "بدون توضیح",
                    SubHomeServiceName = o.Request?.SubHomeService?.Name ?? "نامشخص",
                    FinalPrice = o.FinalPrice,
                    PaymentStatus = o.PaymentStatus,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    OrderDate = o.CreatedAt
                }).ToList();

                _logger.Information("Service: Found {Count} orders for ExpertId: {ExpertId}", result.Count, expertId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Error getting orders for ExpertId: {ExpertId}", expertId);
                return new List<OrderDto>();
            }
        }
    }
}
