using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.DTO.Users.Customers;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Interfaces.IRepository
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByAppUserIdAsync(int appUserId, CancellationToken cancellationToken);
        Task<Customer> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<Proposal> GetProposalByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateProposalAsync(Proposal proposal, CancellationToken cancellationToken);

        Task<decimal> GetBalanceAsync(int customerId, CancellationToken cancellationToken);
        Task<bool> UpdateBalanceAsync(int customerId, decimal newBalance, CancellationToken cancellationToken);

        Task<List<Customer>> GetCustomersByIdsAsync(List<int> customerIds, CancellationToken cancellationToken);
    }
}
