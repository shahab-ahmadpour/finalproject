using App.Domain.Core.DTO.Transactions;
using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Interfaces.IService;
using App.Domain.Core.Transactions.Interfaces.IRepository;
using App.Domain.Core.Transactions.Interfaces.IService;
using App.Domain.Core.Users.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.TransactionServices
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;


        public TransactionService(ITransactionRepository transactionRepository, IUserService userService, IOrderService orderService, ILogger logger)
        {
            _transactionRepository = transactionRepository;
            _userService = userService;
            _orderService = orderService;
            _logger = logger;
        }
        public async Task<TransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching Transaction with ID: {Id}", id);
            var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken);
            return MapToDto(transaction);
        }

        public async Task<List<TransactionDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all Transactions");
            var transactions = await _transactionRepository.GetAllAsync(cancellationToken);
            return transactions.Select(t => MapToDto(t)).ToList(); 
        }

        public async Task<bool> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Creating Transaction for OrderId: {OrderId}", dto.OrderId);
            try
            {
                return await _transactionRepository.CreateAsync(dto, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create Transaction for OrderId: {OrderId}", dto.OrderId);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateTransactionDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating Transaction with ID: {Id}", id);
            return await _transactionRepository.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Deleting Transaction with ID: {Id}", id);
            return await _transactionRepository.DeleteAsync(id, cancellationToken);
        }

        private TransactionDto MapToDto(App.Domain.Core.Transactions.Entities.Transaction transaction)
        {
            if (transaction == null) return null;
            return new TransactionDto
            {
                Id = transaction.Id,
                OrderId = transaction.OrderId,
                CustomerId = transaction.CustomerId,
                ExpertId = transaction.ExpertId,
                Amount = transaction.Amount,
                PaymentMethod = transaction.PaymentMethod,
                PaymentStatus = transaction.PaymentStatus,
                TransactionDate = transaction.TransactionDate,
                CreatedAt = transaction.CreatedAt,
                TransactionType = transaction.TransactionType,
                IsActive = transaction.IsActive
            };
        }
    }
}
