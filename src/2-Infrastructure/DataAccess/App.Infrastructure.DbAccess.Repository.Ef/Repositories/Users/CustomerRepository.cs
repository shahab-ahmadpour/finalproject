using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.DTO.Users.Customers;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Users
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public CustomerRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Customer> GetByAppUserIdAsync(int appUserId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching Customer by AppUserId: {AppUserId}", appUserId);
            try
            {
                var customer = await _dbContext.Customers
                    .Include(c => c.AppUser)
                    .FirstOrDefaultAsync(c => c.AppUserId == appUserId, cancellationToken);
                if (customer == null)
                {
                    _logger.Warning("No Customer found for AppUserId: {AppUserId}", appUserId);
                }
                else
                {
                    _logger.Information("Found Customer with Id: {CustomerId} for AppUserId: {AppUserId}", customer.Id, appUserId);
                }
                return customer;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch Customer for AppUserId: {AppUserId}", appUserId);
                throw;
            }
        }

        public async Task<Customer> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching Customer by Id: {Id}", id);
            try
            {
                var customer = await _dbContext.Customers
                    .Include(c => c.AppUser)
                    .Include(c => c.Orders)
                        .ThenInclude(o => o.Request)
                        .ThenInclude(r => r.SubHomeService)
                    .Include(c => c.Orders)
                        .ThenInclude(o => o.Expert)
                        .ThenInclude(e => e.AppUser)
                    .Include(c => c.Orders)
                        .ThenInclude(o => o.Proposal)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (customer == null)
                {
                    _logger.Warning("No Customer found for Id: {Id}", id);
                }
                else
                {
                    _logger.Information("Found Customer with Id: {Id}, Orders count: {OrdersCount}", customer.Id, customer.Orders?.Count ?? 0);
                }
                return customer;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch Customer for Id: {Id}", id);
                throw;
            }
        }

        public async Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all customers");
            try
            {
                return await _dbContext.Customers
                    .Include(c => c.AppUser)
                    .Select(c => new CustomerDto
                    {
                        Id = c.Id,
                        AppUserId = c.AppUserId,
                        PhoneNumber = c.PhoneNumber,
                        Address = c.Address,
                        City = c.City,
                        State = c.State,
                        Email = c.AppUser != null ? c.AppUser.Email : null,
                        FirstName = c.AppUser != null ? c.AppUser.FirstName : null,
                        LastName = c.AppUser != null ? c.AppUser.LastName : null,
                        ProfilePicture = c.AppUser != null ? c.AppUser.ProfilePicture : null
                    }).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all customers");
                throw;
            }
        }

        public async Task<bool> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating new customer for AppUserId: {AppUserId}", dto.AppUserId);
            try
            {
                var customer = new Customer
                {
                    AppUserId = dto.AppUserId,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State
                };
                await _dbContext.Customers.AddAsync(customer, cancellationToken);
                var result = await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Customer created successfully for AppUserId: {AppUserId}, Result: {Result}", dto.AppUserId, result > 0);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create customer for AppUserId: {AppUserId}", dto.AppUserId);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken)
        {
            _logger.Information("Updating customer with Id: {Id}", customer.Id);
            try
            {
                _dbContext.Customers.Update(customer);
                var result = await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Customer updated successfully for Id: {Id}, Result: {Result}", customer.Id, result > 0);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update customer with Id: {Id}", customer.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting customer with Id: {Id}", id);
            try
            {
                var customer = await GetByIdAsync(id, cancellationToken);
                if (customer == null) return false;

                _dbContext.Customers.Remove(customer);
                var result = await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Customer deleted successfully for Id: {Id}, Result: {Result}", id, result > 0);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete customer with Id: {Id}", id);
                throw;
            }
        }

        public async Task<Proposal> GetProposalByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching proposal with ID: {Id}", id);
            try
            {
                var proposal = await _dbContext.Proposals
                    .Include(p => p.Expert)
                        .ThenInclude(e => e.AppUser)
                    .Include(p => p.Request)
                        .ThenInclude(r => r.SubHomeService)
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                if (proposal == null)
                {
                    _logger.Warning("Proposal with ID: {Id} not found", id);
                    return null;
                }
                _logger.Information("Proposal with ID: {Id} fetched successfully", id);
                return proposal;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch proposal with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateProposalAsync(Proposal proposal, CancellationToken cancellationToken)
        {
            _logger.Information("Updating proposal with ID: {Id}", proposal.Id);
            try
            {
                _dbContext.Proposals.Update(proposal);
                var result = await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Proposal with ID: {Id} updated successfully, Result: {Result}", proposal.Id, result > 0);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update proposal with ID: {Id}", proposal.Id);
                throw;
            }
        }
        public async Task<decimal> GetBalanceAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching balance for CustomerId: {CustomerId}", customerId);
            var customer = await _dbContext.Customers
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

            if (customer == null || customer.AppUser == null)
            {
                _logger.Warning("Customer or AppUser not found for CustomerId: {CustomerId}", customerId);
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            return customer.AppUser.AccountBalance;
        }

        public async Task<bool> UpdateBalanceAsync(int customerId, decimal newBalance, CancellationToken cancellationToken)
        {
            _logger.Information("Updating balance for CustomerId: {CustomerId} to {NewBalance}", customerId, newBalance);
            var customer = await _dbContext.Customers
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

            if (customer == null || customer.AppUser == null)
            {
                _logger.Warning("Customer or AppUser not found for CustomerId: {CustomerId}", customerId);
                return false;
            }

            customer.AppUser.AccountBalance = newBalance;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.Information("Balance updated for CustomerId: {CustomerId}", customerId);
            return true;
        }

        public async Task<List<Customer>> GetCustomersByIdsAsync(List<int> customerIds, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching customers by IDs: {CustomerIds}", string.Join(",", customerIds));

            try
            {
                var customers = await _dbContext.Customers
                    .Include(c => c.AppUser)
                    .Where(c => customerIds.Contains(c.Id))
                    .ToListAsync(cancellationToken);

                _logger.Information("Found {Count} customers from {TotalIds} requested IDs", customers.Count, customerIds.Count);
                return customers;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching customers by IDs");
                throw;
            }
        }
    }
}
