using App.Domain.Core.DTO.Orders;
using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.DTO.Requests;
using App.Domain.Core.DTO.Users.AppUsers;
using App.Domain.Core.DTO.Users.Customers;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Domain.Core.Users.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly IProposalRepository _proposalRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly ILogger _logger;

        public CustomerService(ICustomerRepository customerRepository, IAppUserRepository appUserRepository, IProposalRepository proposalRepository, IRequestRepository requestRepository, ILogger logger)
        {
            _customerRepository = customerRepository;
            _userRepository = appUserRepository;
            _proposalRepository = proposalRepository;
            _requestRepository = requestRepository;
            _logger = logger;
        }

        public async Task<CustomerDto> GetByAppUserIdAsync(int appUserId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching CustomerDto by AppUserId: {AppUserId}", appUserId);
            try
            {
                var customer = await _customerRepository.GetByAppUserIdAsync(appUserId, cancellationToken);
                if (customer == null)
                {
                    _logger.Warning("Service: No customer found for AppUserId: {AppUserId}", appUserId);
                    return null;
                }

                var user = await _userRepository.GetByIdAsync(appUserId, cancellationToken);
                if (user == null)
                {
                    _logger.Warning("Service: No AppUser found for AppUserId: {AppUserId}", appUserId);
                    return null;
                }

                return new CustomerDto
                {
                    Id = customer.Id,
                    AppUserId = customer.AppUserId,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    City = customer.City,
                    State = customer.State,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ProfilePicture = user.ProfilePicture,
                    AccountBalance = user.AccountBalance,
                    IsEnabled = user.IsEnabled,
                    IsConfirmed = user.IsConfirmed,
                    CreatedAt = user.CreatedAt,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch CustomerDto for AppUserId: {AppUserId}", appUserId);
                throw;
            }
        }

        public async Task<CustomerDto> GetByIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching CustomerDto by Id: {Id}", customerId);
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
                if (customer == null)
                {
                    _logger.Warning("Service: No Customer found for Id: {Id}", customerId);
                    return null;
                }

                var user = await _userRepository.GetByIdAsync(customer.AppUserId, cancellationToken);
                if (user == null)
                {
                    _logger.Warning("Service: No AppUser found for AppUserId: {AppUserId}", customer.AppUserId);
                    return null;
                }

                return new CustomerDto
                {
                    Id = customer.Id,
                    AppUserId = customer.AppUserId,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    City = customer.City,
                    State = customer.State,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ProfilePicture = user.ProfilePicture,
                    AccountBalance = user.AccountBalance,
                    IsEnabled = user.IsEnabled,
                    IsConfirmed = user.IsConfirmed,
                    CreatedAt = user.CreatedAt,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch CustomerDto for Id: {Id}", customerId);
                throw;
            }
        }

        public async Task<bool> UpdateCustomerProfileAsync(EditCustomerDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating customer profile for AppUserId: {AppUserId}", dto.AppUserId);
            try
            {
                var customer = await _customerRepository.GetByAppUserIdAsync(dto.AppUserId, cancellationToken);
                if (customer == null)
                {
                    _logger.Warning("Service: Customer not found for update with AppUserId: {AppUserId}", dto.AppUserId);
                    return false;
                }

                var user = await _userRepository.GetByIdAsync(dto.AppUserId, cancellationToken);
                if (user == null)
                {
                    _logger.Warning("Service: AppUser not found for update with AppUserId: {AppUserId}", dto.AppUserId);
                    return false;
                }

                customer.PhoneNumber = dto.PhoneNumber;
                customer.Address = dto.Address;
                customer.City = dto.City;
                customer.State = dto.State;
                var customerUpdated = await _customerRepository.UpdateAsync(customer, cancellationToken);
                _logger.Information("Service: Customer update result: {CustomerUpdated}", customerUpdated);

                string newProfilePicture = dto.ProfilePicture ?? user.ProfilePicture;
                if (dto.ProfilePictureFile != null)
                {
                    try
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ProfilePictureFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await dto.ProfilePictureFile.CopyToAsync(stream);
                        }
                        newProfilePicture = $"/uploads/{fileName}";
                        _logger.Information("Service: New profile picture uploaded: {NewProfilePicture}", newProfilePicture);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Service: Failed to upload profile picture for AppUserId: {AppUserId}", dto.AppUserId);
                        return false;
                    }
                }
                else
                {
                    _logger.Information("Service: No new profile picture, keeping existing: {ExistingProfilePicture}", newProfilePicture);
                }

                var userDto = new UpdateAppUserDto
                {
                    Id = dto.AppUserId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    ProfilePicture = newProfilePicture,
                    IsConfirmed = user.IsConfirmed,
                    Role = user.Role,
                    Password = null
                };
                var userUpdated = await _userRepository.UpdateAsync(dto.AppUserId, userDto, cancellationToken);
                _logger.Information("Service: User update result: {UserUpdated}, ProfilePicture: {ProfilePicture}", userUpdated, userDto.ProfilePicture);

                if (customerUpdated && userUpdated)
                {
                    _logger.Information("Service: Profile updated successfully for AppUserId: {AppUserId}", dto.AppUserId);
                    return true;
                }

                _logger.Warning("Service: Failed to update profile for AppUserId: {AppUserId} (Customer: {CustomerUpdated}, User: {UserUpdated})",
                    dto.AppUserId, customerUpdated, userUpdated);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to update customer profile for AppUserId: {AppUserId}", dto.AppUserId);
                throw;
            }
        }

        public async Task<List<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching orders for CustomerId: {CustomerId}", customerId);
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
                if (customer == null)
                {
                    _logger.Warning("Service: Customer not found for CustomerId: {CustomerId}", customerId);
                    return new List<OrderDto>();
                }

                _logger.Information("Service: Orders count for CustomerId {CustomerId}: {OrdersCount}", customerId, customer.Orders?.Count ?? 0);
                return customer.Orders?.Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer?.AppUser != null ? $"{o.Customer.AppUser.FirstName} {o.Customer.AppUser.LastName}" : "نامشخص",
                    ExpertId = o.ExpertId,
                    ExpertName = o.Expert?.AppUser != null ? $"{o.Expert.AppUser.FirstName} {o.Expert.AppUser.LastName}" : "نامشخص",
                    RequestId = o.RequestId,
                    RequestDescription = o.Request?.Description ?? "نامشخص",
                    SubHomeServiceName = o.Request?.SubHomeService?.Name ?? "نامشخص",
                    ProposalId = o.ProposalId ?? 0,
                    Proposals = o.Proposal != null ? new List<ProposalDto>
            {
                new ProposalDto
                {
                    Id = o.Proposal.Id,
                    ExpertId = o.Proposal.ExpertId,
                    ExpertName = o.Proposal.Expert?.AppUser != null ? $"{o.Proposal.Expert.AppUser.FirstName} {o.Proposal.Expert.AppUser.LastName}" : "نامشخص",
                    OrderId = o.Id,
                    RequestId = o.Proposal.RequestId,
                    RequestDescription = o.Proposal.Request?.Description ?? "نامشخص",
                    SkillId = o.Proposal.SkillId,
                    Price = o.Proposal.Price,
                    ExecutionDate = o.Proposal.ExecutionDate,
                    Description = o.Proposal.Description,
                    Status = o.Proposal.Status,
                    ResponseTime = o.Proposal.ResponseTime,
                    CreatedAt = o.Proposal.CreatedAt,
                    IsEnabled = o.Proposal.IsEnabled,
                    SubHomeServiceName = o.Proposal.Request?.SubHomeService?.Name ?? "نامشخص"
                }
            } : new List<ProposalDto>(),
                    FinalPrice = o.FinalPrice,
                    PaymentStatus = o.PaymentStatus,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    OrderDate = o.CreatedAt,
                    Status = o.PaymentStatus switch
                    {
                        PaymentStatus.Pending => RequestStatus.Pending,
                        PaymentStatus.paid => RequestStatus.InProgress,
                        PaymentStatus.Failed => RequestStatus.Cancelled,
                        PaymentStatus.Completed => RequestStatus.Completed,
                        _ => RequestStatus.Pending
                    },
                    RequestImagePath = o.Request?.EnvironmentImagePath ?? "",
                    ExecutionDate = o.Proposal?.ExecutionDate ?? o.Request?.ExecutionDate ?? DateTime.MinValue 
                }).ToList() ?? new List<OrderDto>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch orders for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<List<ProposalDto>> GetProposalsByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("CustomerService: Fetching proposals for CustomerId: {CustomerId}", customerId);
            try
            {
                var proposalDtos = await _proposalRepository.GetProposalsByCustomerIdAsync(customerId, cancellationToken);
                if (proposalDtos == null || !proposalDtos.Any())
                {
                    _logger.Warning("CustomerService: No proposals found for CustomerId: {CustomerId}", customerId);
                    return new List<ProposalDto>();
                }

                foreach (var dto in proposalDtos)
                {
                    _logger.Information("CustomerService: ProposalDto ID: {Id}, ExpertName: {ExpertName}, SubHomeServiceName: {SubHomeServiceName}, OrderDate: {OrderDate}",
                        dto.Id, dto.ExpertName, dto.SubHomeServiceName, dto.OrderDate == DateTime.MinValue ? "MinValue" : dto.OrderDate.ToString());
                }

                _logger.Information("CustomerService: Fetched {Count} proposals for CustomerId: {CustomerId}", proposalDtos.Count, customerId);
                return proposalDtos;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CustomerService: Failed to fetch proposals for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<Customer> GetCustomerByAppUserIdAsync(int appUserId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching Customer by AppUserId: {AppUserId}", appUserId);
            try
            {
                var customer = await _customerRepository.GetByAppUserIdAsync(appUserId, cancellationToken);
                _logger.Information("Service: Found Customer with Id: {CustomerId} for AppUserId: {AppUserId}", customer?.Id, appUserId);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch Customer for AppUserId: {AppUserId}", appUserId);
                throw;
            }
        }

        public async Task<ProposalDto> GetProposalByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching proposal by ID: {Id}", id);
            try
            {
                var proposal = await _customerRepository.GetProposalByIdAsync(id, cancellationToken);
                if (proposal == null)
                {
                    _logger.Warning("Service: Proposal not found for ID: {Id}", id);
                    return null;
                }

                var proposalDto = new ProposalDto
                {
                    Id = proposal.Id,
                    ExpertId = proposal.ExpertId,
                    ExpertName = proposal.Expert?.AppUser != null ? $"{proposal.Expert.AppUser.FirstName} {proposal.Expert.AppUser.LastName}" : "نامشخص",
                    OrderId = proposal.OrderId,
                    RequestId = proposal.RequestId,
                    RequestDescription = proposal.Request?.Description ?? "نامشخص",
                    SkillId = proposal.SkillId,
                    Price = proposal.Price,
                    ExecutionDate = proposal.ExecutionDate,
                    Description = proposal.Description,
                    Status = proposal.Status,
                    ResponseTime = proposal.ResponseTime,
                    CreatedAt = proposal.CreatedAt,
                    IsEnabled = proposal.IsEnabled,
                    SubHomeServiceName = proposal.Request?.SubHomeService?.Name ?? "نامشخص",
                    PaymentStatus = proposal.Order?.PaymentStatus.ToString() ?? PaymentStatus.Pending.ToString()
                };
                _logger.Information("Service: Proposal with ID: {Id} fetched successfully", id);
                return proposalDto;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch proposal for ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateProposalStatusAsync(int proposalId, ProposalStatus status, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating proposal status for ID: {Id} to {Status}", proposalId, status);
            try
            {
                var proposal = await _customerRepository.GetProposalByIdAsync(proposalId, cancellationToken);
                if (proposal == null)
                {
                    _logger.Warning("Service: Proposal not found for ID: {Id}", proposalId);
                    return false;
                }

                proposal.Status = status;
                var result = await _customerRepository.UpdateProposalAsync(proposal, cancellationToken);
                if (result)
                {
                    _logger.Information("Service: Proposal status updated successfully for ID: {Id} to {Status}", proposalId, status);
                }
                else
                {
                    _logger.Warning("Service: Failed to update proposal status for ID: {Id}", proposalId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to update proposal status for ID: {Id}", proposalId);
                throw;
            }
        }

        public async Task UpdateProposalAsync(Proposal proposal, CancellationToken cancellationToken)
        {
            _logger.Information("CustomerService: Updating proposal with ID: {Id}", proposal.Id);
            await _proposalRepository.UpdateAsync(proposal, cancellationToken);
            _logger.Information("CustomerService: Successfully updated proposal with ID: {Id}", proposal.Id);
        }

        public async Task<Proposal> GetFullProposalByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("CustomerService: Fetching full proposal with ID: {Id}", id);
            var proposal = await _proposalRepository.GetByIdAsync(id, cancellationToken);
            return proposal;
        }

        public async Task<RequestDto> GetRequestByIdAsync(int requestId, CancellationToken cancellationToken)
        {
            _logger.Information("CustomerService: Fetching request by ID: {Id}", requestId);
            var request = await _requestRepository.GetAsync(requestId, cancellationToken);
            if (request == null)
            {
                _logger.Warning("CustomerService: Request not found for ID: {Id}", requestId);
                return null;
            }

            return new RequestDto
            {
                Id = request.Id,
                CustomerId = request.CustomerId,
                SubHomeServiceId = request.SubHomeServiceId,
                SubHomeServiceName = request.SubHomeServiceName,
                Description = request.Description,
                Status = request.Status,
                Deadline = request.Deadline,
                ExecutionDate = request.ExecutionDate,
                EnvironmentImagePath = request.EnvironmentImagePath
            };
        }
        public async Task<decimal> GetBalanceAsync(int customerId, CancellationToken cancellationToken)
        {
            return await _customerRepository.GetBalanceAsync(customerId, cancellationToken);
        }

        public async Task<bool> UpdateBalanceAsync(int customerId, decimal newBalance, CancellationToken cancellationToken)
        {
            return await _customerRepository.UpdateBalanceAsync(customerId, newBalance, cancellationToken);
        }
        public async Task<List<Customer>> GetCustomersByIdsAsync(List<int> customerIds, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching customers for IDs: {CustomerIds}", string.Join(",", customerIds));
            try
            {
                var customers = await _customerRepository.GetCustomersByIdsAsync(customerIds, cancellationToken);
                if (customers == null || !customers.Any())
                {
                    _logger.Warning("No customers found for IDs: {CustomerIds}", string.Join(",", customerIds));
                    return new List<Customer>();
                }
                return customers;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching customers for IDs: {CustomerIds}", string.Join(",", customerIds));
                throw;
            }
        }
    }
}

