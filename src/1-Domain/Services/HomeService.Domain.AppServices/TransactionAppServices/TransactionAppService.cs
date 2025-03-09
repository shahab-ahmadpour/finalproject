using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using App.Domain.Core.Transactions.Interfaces.IAppService;
using App.Domain.Core.Transactions.Interfaces.IRepository;
using App.Domain.Core.Transactions.Interfaces.IService;
using App.Domain.Core.Users.Interfaces.IRepository;
using Serilog;
using App.Domain.Core.Users.Interfaces.IService;
using HomeService.Domain.Services.OrderServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using App.Domain.Core.Transactions.Entities;
using App.Domain.Core.DTO.Transactions;
using HomeService.Domain.Services.TransactionServices;
using App.Domain.Core.DTO.Orders;
using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IAppService;
using HomeService.Domain.AppServices.OrderAppServices;

namespace HomeService.Domain.AppServices.TransactionAppServices
{
    public class TransactionAppService : ITransactionAppService
    {
        private readonly IOrderAppService _orderAppService;
        private readonly ICustomerAppService _customerAppService;
        private readonly IExpertAppService _expertAppService;
        private readonly IAppUserRepository _userRepository;
        private readonly ITransactionService _transactionService;
        private readonly Serilog.ILogger _logger;

        public TransactionAppService(
            IOrderAppService orderAppService,
            IAppUserRepository userRepository,
            IExpertAppService expertAppService,
            ICustomerAppService customerAppService,
            ITransactionService transactionService,
        Serilog.ILogger logger
            )
        {
            _orderAppService= orderAppService;
            _customerAppService = customerAppService;
            _expertAppService = expertAppService;
            _userRepository = userRepository;
            _transactionService = transactionService;
            _logger = logger;
        }

        public async Task<TransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching Transaction with ID: {Id}", id);
            return await _transactionService.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<TransactionDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all Transactions");
            return await _transactionService.GetAllAsync(cancellationToken);
        }

        public async Task<bool> ProcessPaymentAsync(int orderId, int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Starting payment process for OrderId: {OrderId}, CustomerId: {CustomerId}", orderId, customerId);

            var order = await _orderAppService.GetAsync(orderId, cancellationToken);
            if (order == null)
            {
                _logger.Warning("Order {OrderId} not found", orderId);
                return false;
            }
            _logger.Information("Order found: OrderId: {OrderId}, CustomerId: {CustomerId}, ExpertId: {ExpertId}, FinalPrice: {FinalPrice}", order.Id, order.CustomerId, order.ExpertId, order.FinalPrice);

            if (order.CustomerId != customerId)
            {
                _logger.Warning("Order {OrderId} does not belong to CustomerId: {CustomerId}", orderId, customerId);
                return false;
            }

            var customerBalance = await _customerAppService.GetBalanceAsync(customerId, cancellationToken);
            _logger.Information("Customer balance: {Balance} for CustomerId: {CustomerId}", customerBalance, customerId);
            if (customerBalance < order.FinalPrice)
            {
                _logger.Warning("Insufficient balance for CustomerId: {CustomerId}, Balance: {Balance}, Required: {Amount}",
                    customerId, customerBalance, order.FinalPrice);
                return false;
            }

            var newCustomerBalance = customerBalance - order.FinalPrice;
            if (!await _customerAppService.UpdateBalanceAsync(customerId, newCustomerBalance, cancellationToken))
            {
                _logger.Error("Failed to update customer balance for CustomerId: {CustomerId}", customerId);
                return false;
            }
            _logger.Information("Customer balance updated to: {NewBalance} for CustomerId: {CustomerId}", newCustomerBalance, customerId);

            var expertBalance = await _expertAppService.GetBalanceAsync(order.ExpertId, cancellationToken);
            var newExpertBalance = expertBalance + order.FinalPrice;
            if (!await _expertAppService.UpdateBalanceAsync(order.ExpertId, newExpertBalance, cancellationToken))
            {
                _logger.Error("Failed to update expert balance for ExpertId: {ExpertId}", order.ExpertId);
                return false;
            }
            _logger.Information("Expert balance updated to: {NewBalance} for ExpertId: {ExpertId}", newExpertBalance, order.ExpertId);

            var transactionDto = new CreateTransactionDto
            {
                OrderId = orderId,
                CustomerId = customerId,
                ExpertId = order.ExpertId,
                Amount = order.FinalPrice,
                PaymentMethod = "Default",
                PaymentStatus = PaymentStatus.Completed,
                TransactionDate = DateTime.UtcNow,
                TransactionType = TransactionType.Withdrawal,
                IsActive = true
            };
            _logger.Information("Creating transaction with OrderId: {OrderId}, CustomerId: {CustomerId}, ExpertId: {ExpertId}, Amount: {Amount}", orderId, customerId, order.ExpertId, order.FinalPrice);
            if (!await _transactionService.CreateAsync(transactionDto, cancellationToken))
            {
                _logger.Error("Failed to create transaction for OrderId: {OrderId}", orderId);
                return false;
            }
            _logger.Information("Transaction created successfully for OrderId: {OrderId}", orderId);

            if (!await _orderAppService.UpdatePaymentStatusAsync(orderId, PaymentStatus.Completed, cancellationToken))
            {
                _logger.Error("Failed to update payment status for OrderId: {OrderId}", orderId);
                return false;
            }
            _logger.Information("Payment status updated to Completed for OrderId: {OrderId}", orderId);

            _logger.Information("Payment process completed successfully for OrderId: {OrderId}, CustomerId: {CustomerId}", orderId, customerId);
            return true;
        }

        public async Task<bool> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating Transaction for OrderId: {OrderId}", dto.OrderId);
            return await _transactionService.CreateAsync(dto, cancellationToken);
        }

        public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating Transaction with ID: {Id}", id);
            return await _transactionService.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteTransactionAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting Transaction with ID: {Id}", id);
            return await _transactionService.DeleteAsync(id, cancellationToken);
        }
    }
}