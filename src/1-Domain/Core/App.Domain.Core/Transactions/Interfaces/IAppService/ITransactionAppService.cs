using App.Domain.Core.DTO.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Transactions.Interfaces.IAppService
{
    public interface ITransactionAppService
    {
        Task<TransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<TransactionDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> ProcessPaymentAsync(int orderId, int customerId, CancellationToken cancellationToken);
        Task<bool> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteTransactionAsync(int id, CancellationToken cancellationToken);
    }
}
