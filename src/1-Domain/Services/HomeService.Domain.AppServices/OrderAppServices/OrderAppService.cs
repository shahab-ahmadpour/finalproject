using App.Domain.Core.DTO.Orders;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IService;
using App.Domain.Core.Users.Interfaces.IAppService;
using HomeService.Domain.AppServices.ProposalAppServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.AppServices.OrderAppServices
{
    public class OrderAppService : IOrderAppService
    {
        private readonly IOrderService _orderService;
        private readonly IProposalAppService _proposalAppService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public OrderAppService(
            IOrderService orderService,
            IProposalAppService proposalAppService,
            ILogger logger,
            IMemoryCache memoryCache)
        {
            _orderService = orderService;
            _proposalAppService = proposalAppService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<bool> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Creating new order.");
            var result = await _orderService.CreateAsync(dto, cancellationToken);
            if (result)
            {
                _memoryCache.Remove($"Orders_Customer_{dto.CustomerId}");
                _memoryCache.Remove($"Orders_Expert_{dto.ExpertId}");
            }
            return result;
        }

        public async Task<bool> UpdateAsync(int id, UpdateOrderDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating order with Id: {Id}", id);
            var result = await _orderService.UpdateAsync(id, dto, cancellationToken);

            if (!result)
            {
                _logger.Warning("AppService: Failed to update order with Id: {Id}. Order not found.", id);
            }

            return result;
        }


        public async Task<OrderDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching order with Id: {Id}", id);
            return await _orderService.GetAsync(id, cancellationToken);
        }

        public async Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all orders.");
            return await _orderService.GetAllAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Deleting (Deactivating) order with Id: {Id}", id);
            return await _orderService.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatus status, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating payment status for order Id: {Id} to {Status}", id, status);
            return await _orderService.UpdatePaymentStatusAsync(id, status, cancellationToken);
        }

        public async Task<UpdateOrderDto> GetOrderForEditAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Preparing order with Id: {Id} for editing.", id);

            var order = await _orderService.GetAsync(id, cancellationToken);
            if (order == null)
            {
                _logger.Warning("AppService: Order with Id: {Id} not found.", id);
                return null;
            }

            var dto = new UpdateOrderDto
            {
                Id = order.Id,
                PaymentStatus = order.PaymentStatus,
                IsActive = order.IsActive,
                CustomerName = order.CustomerName,
                ExpertName = order.ExpertName,
                RequestDescription = order.RequestDescription
            };

            _logger.Information("AppService: Order with Id: {Id} prepared for editing.", id);

            return dto;
        }

        public async Task<List<OrderDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching orders for CustomerId: {CustomerId} in OrderAppService", customerId);
            string cacheKey = $"Orders_Customer_{customerId}";

            if (!_memoryCache.TryGetValue(cacheKey, out List<OrderDto> cachedOrders))
            {
                _logger.Information("Orders not found in cache for CustomerId: {CustomerId}, fetching from database", customerId);
                try
                {
                    cachedOrders = await _orderService.GetByCustomerIdAsync(customerId, cancellationToken);
                    if (cachedOrders == null || !cachedOrders.Any())
                    {
                        _logger.Warning("No orders retrieved for CustomerId: {CustomerId} in OrderAppService", customerId);
                    }
                    else
                    {
                        _logger.Information("Successfully retrieved {OrderCount} orders for CustomerId: {CustomerId} from database", cachedOrders.Count, customerId);
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        };
                        _memoryCache.Set(cacheKey, cachedOrders, cacheOptions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error fetching orders for CustomerId: {CustomerId} in OrderAppService", customerId);
                    throw;
                }
            }
            else
            {
                _logger.Information("Orders retrieved from cache for CustomerId: {CustomerId}, Count: {OrderCount}", customerId, cachedOrders.Count);
            }
            return cachedOrders;
        }

        public async Task<bool> CreateOrderFromProposalAsync(int proposalId, int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Creating order from proposal ID: {ProposalId} for CustomerId: {CustomerId}", proposalId, customerId);
            try
            {
                var proposal = await _orderService.GetAsync(proposalId, cancellationToken);
                if (proposal == null)
                {
                    _logger.Warning("AppService: Proposal not found for ID: {ProposalId}", proposalId);
                    return false;
                }

                var createOrderDto = new CreateOrderDto
                {
                    CustomerId = customerId,
                    ExpertId = proposal.ExpertId,
                    RequestId = proposal.RequestId,
                    ProposalId = proposalId,
                    FinalPrice = proposal.FinalPrice,
                    PaymentStatus = PaymentStatus.Pending
                };

                var result = await _orderService.CreateAsync(createOrderDto, cancellationToken);
                if (result)
                {
                    _logger.Information("AppService: Order created successfully from proposal ID: {ProposalId}", proposalId);
                }
                else
                {
                    _logger.Warning("AppService: Failed to create order from proposal ID: {ProposalId}", proposalId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AppService: Failed to create order from proposal ID: {ProposalId}", proposalId);
                throw;
            }
        }
        public async Task<int> ProcessProposalSelectionAsync(int proposalId, int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("OrderAppService: Processing selection for proposal ID: {ProposalId} for CustomerId: {CustomerId}", proposalId, customerId);
            try
            {
                var proposalDto = await _proposalAppService.GetProposalByIdAsync(proposalId, cancellationToken);
                if (proposalDto == null || proposalDto.Status != ProposalStatus.Accepted)
                {
                    _logger.Warning("OrderAppService: Proposal {Id} not found or not accepted", proposalId);
                    throw new InvalidOperationException("پیشنهاد یافت نشد یا قابل انتخاب نیست.");
                }

                var createOrderDto = new CreateOrderDto
                {
                    CustomerId = customerId,
                    ExpertId = proposalDto.ExpertId,
                    RequestId = proposalDto.RequestId,
                    ProposalId = proposalId,
                    FinalPrice = proposalDto.Price,
                    PaymentStatus = PaymentStatus.Pending
                };

                var result = await _orderService.CreateAsync(createOrderDto, cancellationToken);
                if (!result)
                {
                    _logger.Warning("OrderAppService: Failed to create order from proposal ID: {ProposalId}", proposalId);
                    throw new InvalidOperationException("ایجاد سفارش با خطا مواجه شد. لطفاً دیتای ExpertId و RequestId را بررسی کنید.");
                }

                var order = await _orderService.GetByProposalIdAsync(proposalId, cancellationToken);
                if (order == null)
                {
                    _logger.Error("OrderAppService: Newly created order not found for ProposalId: {ProposalId}", proposalId);
                    throw new InvalidOperationException("سفارش ایجاد شده یافت نشد. لطفاً دیتابیس را بررسی کنید.");
                }

                var proposalToUpdate = new Proposal
                {
                    Id = proposalDto.Id,
                    OrderId = order.Id,
                    Status = proposalDto.Status,
                    IsEnabled = proposalDto.IsEnabled
                };
                await _proposalAppService.UpdateProposalAsync(proposalToUpdate, cancellationToken);

                _logger.Information("OrderAppService: Order created successfully for ProposalId: {ProposalId}, OrderId: {OrderId}", proposalId, order.Id);
                return order.Id;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OrderAppService: Failed to process selection for proposal ID: {ProposalId}", proposalId);
                throw;
            }
        }
        public async Task<Order> GetByProposalIdAsync(int proposalId, CancellationToken cancellationToken)
        {
            _logger.Information("OrderAppService: Fetching order by ProposalId: {ProposalId}", proposalId);
            var order = await _orderService.GetByProposalIdAsync(proposalId, cancellationToken);
            return order;
        }

        public async Task<List<OrderDto>> GetOrdersByExpertIdAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching orders for ExpertId: {ExpertId}", expertId);
            string cacheKey = $"Orders_Expert_{expertId}";

            if (!_memoryCache.TryGetValue(cacheKey, out List<OrderDto> cachedOrders))
            {
                _logger.Information("Orders not found in cache for ExpertId: {ExpertId}, fetching from database", expertId);
                try
                {
                    var orders = await _orderService.GetOrdersByExpertIdAsync(expertId, cancellationToken);
                    if (orders == null || !orders.Any())
                    {
                        _logger.Warning("No orders retrieved for ExpertId: {ExpertId} in OrderAppService", expertId);
                        return new List<OrderDto>();
                    }

                    _logger.Information("Successfully retrieved {OrderCount} orders for ExpertId: {ExpertId} from database", orders.Count, expertId);
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    };
                    _memoryCache.Set(cacheKey, orders, cacheOptions);

                    return orders;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error fetching orders for ExpertId: {ExpertId} in OrderAppService", expertId);
                    return new List<OrderDto>();
                }
            }
            else
            {
                _logger.Information("Orders retrieved from cache for ExpertId: {ExpertId}, Count: {OrderCount}", expertId, cachedOrders.Count);
                return cachedOrders;
            }
        }
    }
}