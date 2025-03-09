using App.Domain.Core.DTO.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Transactions.Interfaces.IService
{
    public interface ITransactionService
    {
        Task<TransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<TransactionDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateTransactionDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
