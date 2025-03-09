using App.Domain.Core.DTO.Transactions;
using App.Domain.Core.Transactions.Entities;
using App.Domain.Core.Transactions.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Transactions
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public TransactionRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Transaction> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching Transaction with ID: {Id}", id);
            return await _dbContext.Set<Transaction>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all Transactions");
            return await _dbContext.Set<Transaction>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken)
        {
            if (dto == null)
            {
                _logger.Error("CreateTransactionDto is null");
                return false;
            }

            if (_dbContext == null)
            {
                _logger.Error("AppDbContext is null");
                throw new InvalidOperationException("DbContext is not initialized");
            }

            if (_dbContext.Transactions == null)
            {
                _logger.Error("DbSet<Transactions> is not configured in AppDbContext");
                throw new InvalidOperationException("Transactions DbSet is not configured");
            }

            var transaction = new Transaction
            {
                OrderId = dto.OrderId,
                CustomerId = dto.CustomerId,
                ExpertId = dto.ExpertId,
                Amount = dto.Amount,
                PaymentMethod = string.IsNullOrEmpty(dto.PaymentMethod) ? "Default" : dto.PaymentMethod,
                PaymentStatus = dto.PaymentStatus,
                TransactionDate = dto.TransactionDate,
                TransactionType = dto.TransactionType,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _dbContext.Transactions.Add(transaction);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Transaction created successfully for OrderId: {OrderId}", dto.OrderId);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex, "Database error while saving Transaction for OrderId: {OrderId}", dto.OrderId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error while saving Transaction for OrderId: {OrderId}", dto.OrderId);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateTransactionDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating Transaction with ID: {Id}", id);
            var transaction = await _dbContext.Set<Transaction>()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            if (transaction == null)
            {
                _logger.Warning("Transaction not found for ID: {Id}", id);
                return false;
            }

            transaction.PaymentStatus = dto.PaymentStatus;

            _dbContext.Set<Transaction>().Update(transaction);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting Transaction with ID: {Id}", id);
            var transaction = await _dbContext.Set<Transaction>()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            if (transaction == null)
            {
                _logger.Warning("Transaction not found for ID: {Id}", id);
                return false;
            }

            _dbContext.Set<Transaction>().Remove(transaction);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
