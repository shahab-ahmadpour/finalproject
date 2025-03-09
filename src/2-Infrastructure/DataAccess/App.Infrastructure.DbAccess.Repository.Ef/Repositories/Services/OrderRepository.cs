using App.Domain.Core.DTO.Orders;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public OrderRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("OrderRepository: Creating new order with ProposalId: {ProposalId}", dto.ProposalId);
            try
            {
                var proposal = await _dbContext.Proposals
                    .FirstOrDefaultAsync(p => p.Id == dto.ProposalId, cancellationToken);
                if (proposal == null)
                {
                    _logger.Warning("OrderRepository: Proposal with ID: {ProposalId} not found for order creation", dto.ProposalId);
                    return false;
                }

                var customer = await _dbContext.Customers
                    .FirstOrDefaultAsync(c => c.Id == dto.CustomerId, cancellationToken);
                if (customer == null)
                {
                    _logger.Warning("OrderRepository: Customer with ID: {CustomerId} not found for order creation", dto.CustomerId);
                    return false;
                }

                var expert = await _dbContext.Experts
                    .FirstOrDefaultAsync(e => e.Id == dto.ExpertId, cancellationToken);
                if (expert == null)
                {
                    _logger.Warning("OrderRepository: Expert with ID: {ExpertId} not found for order creation", dto.ExpertId);
                    return false;
                }

                var request = await _dbContext.Requests
                    .FirstOrDefaultAsync(r => r.Id == dto.RequestId, cancellationToken);
                if (request == null)
                {
                    _logger.Warning("OrderRepository: Request with ID: {RequestId} not found for order creation", dto.RequestId);
                    return false;
                }

                var order = new Order
                {
                    CustomerId = dto.CustomerId,
                    ExpertId = dto.ExpertId,
                    RequestId = dto.RequestId,
                    ProposalId = dto.ProposalId,
                    Proposal = proposal,
                    FinalPrice = dto.FinalPrice,
                    PaymentStatus = dto.PaymentStatus,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("OrderRepository: Successfully created order with Id: {Id} for ProposalId: {ProposalId}", order.Id, dto.ProposalId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OrderRepository: Failed to create order with ProposalId: {ProposalId}. Error: {ErrorMessage}", dto.ProposalId, ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateOrderDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating order with ID: {Id}", id);

            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            if (order == null)
            {
                _logger.Warning("Order with ID: {Id} not found", id);
                return false;
            }

            order.PaymentStatus = dto.PaymentStatus;
            order.IsActive = dto.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.Information("Successfully updated order with ID: {Id}", id);
            return true;
        }

        public async Task<List<Order>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching orders for CustomerId: {CustomerId}", customerId);
            try
            {
                var orders = await _dbContext.Orders
                    .Include(o => o.Request)
                    .Include(o => o.Expert)
                    .ThenInclude(e => e.AppUser)
                    .Include(o => o.Proposal)
                    .Where(o => o.CustomerId == customerId && o.IsActive)
                    .ToListAsync(cancellationToken);

                if (orders == null || !orders.Any())
                {
                    _logger.Warning("No active orders found for CustomerId: {CustomerId}", customerId);
                }
                else
                {
                    _logger.Information("Retrieved {OrderCount} active orders for CustomerId: {CustomerId}", orders.Count, customerId);
                }

                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch orders for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<OrderDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders
                    .Include(o => o.Customer)
                        .ThenInclude(c => c.AppUser)
                    .Include(o => o.Expert)
                        .ThenInclude(e => e.AppUser)
                    .Include(o => o.Request)
                        .ThenInclude(r => r.SubHomeService)
                    .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                ExpertId = order.ExpertId,
                RequestId = order.RequestId,
                FinalPrice = order.FinalPrice,
                PaymentStatus = order.PaymentStatus,
                IsActive = order.IsActive,
                CreatedAt = order.CreatedAt,
                CustomerName = order.Customer?.AppUser?.FirstName + " " + order.Customer?.AppUser?.LastName ?? "نامشخص",
                SubHomeServiceName = order.Request.SubHomeService.Name,
                ExpertName = order.Expert?.AppUser?.FirstName + " " + order.Expert?.AppUser?.LastName ?? "نامشخص",
                RequestDescription = order.Request?.Description ?? "بدون توضیح"
            };
        }

        public async Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all orders.");
            var orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Expert)
                .Include(o => o.Request)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.AppUser.FirstName + " " + o.Customer.AppUser.LastName,
                    ExpertId = o.ExpertId,
                    ExpertName = o.Expert.AppUser.FirstName + " " + o.Expert.AppUser.LastName,
                    RequestId = o.RequestId,
                    RequestDescription = o.Request.Description,
                    FinalPrice = o.FinalPrice,
                    PaymentStatus = o.PaymentStatus,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);

            _logger.Information("Fetched {Count} orders.", orders.Count);
            return orders;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting (deactivating) order with ID: {Id}", id);
            try
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
                if (order == null)
                {
                    _logger.Warning("Order with ID: {Id} not found for deletion.", id);
                    return false;
                }

                order.IsActive = false;
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully deactivated order with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deactivate order with ID: {Id}", id);
                return false;
            }
        }

        public async Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatus status, CancellationToken cancellationToken)
        {
            _logger.Information("Updating payment status for order ID: {Id} to {Status}", id, status);

            try
            {
                var order = await _dbContext.Orders
                    .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

                if (order == null)
                {
                    _logger.Warning("Order with ID: {Id} not found for payment status update.", id);
                    return false;
                }

                order.PaymentStatus = status;
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.Information("Successfully updated payment status for order ID: {Id} to {Status}", id, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update payment status for order ID: {Id}", id);
                return false;
            }
        }


        public async Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            _logger.Information("Fetching all orders asynchronously.");
            try
            {
                var orders = await _dbContext.Orders.ToListAsync(cancellationToken);
                _logger.Information("Fetched {Count} orders asynchronously.", orders?.Count ?? 0);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all orders asynchronously.");
                throw;
            }
        }

        public async Task<Order> GetByProposalIdAsync(int proposalId, CancellationToken cancellationToken)
        {
            _logger.Information("OrderRepository: Fetching order by ProposalId: {ProposalId}", proposalId);
            var order = await _dbContext.Orders
                .Include(o => o.Proposal)
                .Where(o => o.ProposalId == proposalId && o.IsActive)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                _logger.Warning("OrderRepository: Order not found for ProposalId: {ProposalId}", proposalId);
            }
            else
            {
                _logger.Information("OrderRepository: Successfully fetched order with ID: {OrderId} for ProposalId: {ProposalId}", order.Id, proposalId);
            }

            return order;
        }

        public async Task<List<Order>> GetByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("OrderRepository: Fetching orders for ExpertId: {ExpertId}", expertId);

            try
            {
                var orders = await _dbContext.Orders
                    .Include(o => o.Customer)
                        .ThenInclude(c => c.AppUser)
                    .Include(o => o.Expert)
                        .ThenInclude(e => e.AppUser)
                    .Include(o => o.Request)
                        .ThenInclude(r => r.SubHomeService)
                    .Include(o => o.Proposal)
                    .Where(o => o.ExpertId == expertId && o.IsActive)
                    .ToListAsync(cancellationToken);

                if (orders == null || !orders.Any())
                {
                    _logger.Warning("OrderRepository: No orders found for ExpertId: {ExpertId}", expertId);
                    return new List<Order>();
                }

                _logger.Information("OrderRepository: Retrieved {Count} orders for ExpertId: {ExpertId}", orders.Count, expertId);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OrderRepository: Failed to fetch orders for ExpertId: {ExpertId}", expertId);
                return new List<Order>();
            }
        }
    }
}