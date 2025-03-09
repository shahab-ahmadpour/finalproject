using App.Domain.Core.DTO.Orders;
using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.DTO.Requests;
using App.Domain.Core.DTO.Users.Customers;
using App.Domain.Core.DTO.Users.Customers;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Interfaces.IService
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetByAppUserIdAsync(int appUserId, CancellationToken cancellationToken);
        Task<CustomerDto> GetByIdAsync(int customerId, CancellationToken cancellationToken); 
        Task<bool> UpdateCustomerProfileAsync(EditCustomerDto dto, CancellationToken cancellationToken);
        Task<List<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
        Task<List<ProposalDto>> GetProposalsByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
        Task<Customer> GetCustomerByAppUserIdAsync(int appUserId, CancellationToken cancellationToken);
        Task<ProposalDto> GetProposalByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateProposalStatusAsync(int proposalId, ProposalStatus status, CancellationToken cancellationToken);
        Task UpdateProposalAsync(Proposal proposal, CancellationToken cancellationToken);
        Task<Proposal> GetFullProposalByIdAsync(int id, CancellationToken cancellationToken);
        Task<RequestDto> GetRequestByIdAsync(int requestId, CancellationToken cancellationToken);
        Task<decimal> GetBalanceAsync(int customerId, CancellationToken cancellationToken);
        Task<bool> UpdateBalanceAsync(int customerId, decimal newBalance, CancellationToken cancellationToken);
        Task<List<Customer>> GetCustomersByIdsAsync(List<int> customerIds, CancellationToken cancellationToken);
    }
}
