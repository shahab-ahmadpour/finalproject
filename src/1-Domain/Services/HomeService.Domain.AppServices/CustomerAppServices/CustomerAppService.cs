using App.Domain.Core.DTO.Categories;
using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.DTO.Orders;
using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.DTO.Requests;
using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.DTO.Users.Customers;
using App.Domain.Core.Enums;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IAppService;
using App.Domain.Core.Users.Interfaces.IService;
using Microsoft.Extensions.Caching.Memory;
using Serilog;




namespace HomeService.Domain.AppServices.CustomerAppServices
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly ICustomerService _customerService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IHomeServiceAppService _homeServiceAppService;
        private readonly ISubHomeServiceAppService _subHomeServiceAppService;
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public CustomerAppService(
            ICustomerService customerService,
            ICategoryAppService categoryAppService,
            IHomeServiceAppService homeServiceAppService,
            ISubHomeServiceAppService subHomeServiceAppService,
            IOrderAppService orderAppService,
            ILogger logger,
            IMemoryCache cache)
        {
            _customerService = customerService;
            _categoryAppService = categoryAppService;
            _homeServiceAppService = homeServiceAppService;
            _subHomeServiceAppService = subHomeServiceAppService;
            _orderAppService = orderAppService;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching Customer by CustomerId: {CustomerId}", customerId);
            try
            {
                return await _customerService.GetByIdAsync(customerId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AppService: Failed to fetch Customer for CustomerId: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<CustomerDto> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching customer by CustomerId: {CustomerId}", customerId);
            var customer = await _customerService.GetByIdAsync(customerId, cancellationToken);
            if (customer == null)
            {
                _logger.Warning("AppService: Customer not found for CustomerId: {CustomerId}", customerId);
                return null;
            }

            return customer;
        }

        public async Task<EditCustomerDto> GetEditCustomerProfileAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching edit customer profile for CustomerId: {CustomerId}", customerId);
            var customer = await _customerService.GetByIdAsync(customerId, cancellationToken);
            if (customer == null)
            {
                _logger.Warning("AppService: Customer not found for CustomerId: {CustomerId}", customerId);
                return null;
            }

            return new EditCustomerDto
            {
                AppUserId = customer.AppUserId,
                FirstName = customer.FirstName ?? "N/A",
                LastName = customer.LastName ?? "N/A",
                ProfilePicture = customer.ProfilePicture ?? "default.png",
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                State = customer.State
            };
        }

        public async Task<bool> UpdateCustomerProfileAsync(EditCustomerDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating customer profile for AppUserId: {AppUserId}", dto.AppUserId);
            var result = await _customerService.UpdateCustomerProfileAsync(dto, cancellationToken);
            if (result)
            {
                _logger.Information("AppService: Customer profile updated successfully for AppUserId: {AppUserId}", dto.AppUserId);
            }
            else
            {
                _logger.Warning("AppService: Failed to update customer profile for AppUserId: {AppUserId}", dto.AppUserId);
            }
            return result;
        }

        public async Task<List<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching orders for CustomerId: {CustomerId}", customerId);
            return await _customerService.GetOrdersByCustomerIdAsync(customerId, cancellationToken);
        }

        public async Task<List<ProposalDto>> GetProposalsByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching proposals for CustomerId: {CustomerId}", customerId);
            return await _customerService.GetProposalsByCustomerIdAsync(customerId, cancellationToken);
        }

        public async Task<Customer> GetCustomerByAppUserIdAsync(int appUserId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching Customer by AppUserId: {AppUserId}", appUserId);
            try
            {
                var customer = await _customerService.GetCustomerByAppUserIdAsync(appUserId, cancellationToken);
                _logger.Information("AppService: Found Customer with Id: {CustomerId} for AppUserId: {AppUserId}", customer?.Id, appUserId);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AppService: Failed to fetch Customer for AppUserId: {AppUserId}", appUserId);
                throw;
            }
        }

        public async Task<ServiceHierarchyViewModel> GetServiceSelectionDataAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching service selection data");
            var cacheKey = "ServiceSelectionData";
            if (!_cache.TryGetValue(cacheKey, out ServiceHierarchyViewModel model))
            {
                var categories = await _categoryAppService.GetAllCategoriesAsync(cancellationToken);
                var homeServices = await _homeServiceAppService.GetAllHomeServicesAsync(cancellationToken);
                var subHomeServices = await _subHomeServiceAppService.GetSubHomeServicesAsync(cancellationToken);
                model = new ServiceHierarchyViewModel
                {
                    Categories = categories ?? new List<CategoryDto>(),
                    HomeServicesByCategory = homeServices?.GroupBy(hs => hs.CategoryId)
                        .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<HomeServiceDto>>(),
                    SubHomeServicesByHomeService = subHomeServices?.GroupBy(s => s.HomeServiceId)
                        .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<SubHomeServiceListItemDto>>()
                };
                _cache.Set(cacheKey, model, TimeSpan.FromMinutes(30));
            }
            return model;
        }

        public async Task<ServiceHierarchyViewModel> GetServicesByCategoryAsync(int categoryId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching services by CategoryId: {CategoryId}", categoryId);
            var cacheKey = $"ServicesByCategory_{categoryId}";
            if (!_cache.TryGetValue(cacheKey, out ServiceHierarchyViewModel model))
            {
                var homeServices = await _homeServiceAppService.GetAllHomeServicesAsync(cancellationToken);
                var subHomeServices = await _subHomeServiceAppService.GetSubHomeServicesAsync(cancellationToken);
                var filteredHomeServices = homeServices?.Where(hs => hs.CategoryId == categoryId).ToList() ?? new List<HomeServiceDto>();
                var filteredSubHomeServices = subHomeServices?.Where(s => filteredHomeServices.Any(hs => hs.Id == s.HomeServiceId)).ToList() ?? new List<SubHomeServiceListItemDto>();
                model = new ServiceHierarchyViewModel
                {
                    HomeServicesByCategory = filteredHomeServices.GroupBy(hs => hs.CategoryId)
                        .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<HomeServiceDto>>(),
                    SubHomeServicesByHomeService = filteredSubHomeServices.GroupBy(s => s.HomeServiceId)
                        .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<SubHomeServiceListItemDto>>()
                };
                _cache.Set(cacheKey, model, TimeSpan.FromMinutes(30));
            }
            return model;
        }

        public async Task<CreateRequestDto> PrepareCreateRequestAsync(int customerId, int subHomeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Preparing create request for CustomerId: {CustomerId}, SubHomeServiceId: {SubHomeServiceId}", customerId, subHomeServiceId);
            var customerDto = await GetByCustomerIdAsync(customerId, cancellationToken);
            if (customerDto == null)
            {
                _logger.Warning("AppService: Customer not found for CustomerId: {CustomerId}", customerId);
                return null;
            }

            var subHomeService = await _subHomeServiceAppService.GetSubHomeServiceByIdAsync(subHomeServiceId, cancellationToken);
            if (subHomeService == null)
            {
                _logger.Warning("AppService: SubHomeService not found for Id: {SubHomeServiceId}", subHomeServiceId);
                return null;
            }

            return new CreateRequestDto
            {
                CustomerId = customerId,
                SubHomeServiceId = subHomeServiceId,
                SubHomeServiceName = subHomeService.Name
            };
        }

        public async Task UpdateProposalAsync(Proposal proposal, CancellationToken cancellationToken)
        {
            _logger.Information("CustomerAppService: Updating proposal with ID: {Id}", proposal.Id);
            await _customerService.UpdateProposalAsync(proposal, cancellationToken);
            _logger.Information("CustomerAppService: Successfully updated proposal with ID: {Id}", proposal.Id);
        }

        public async Task<ProposalDto> GetProposalByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching proposal by ID: {Id}", id);
            return await _customerService.GetProposalByIdAsync(id, cancellationToken);
        }

        public async Task<bool> UpdateProposalStatusAsync(int proposalId, ProposalStatus status, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Updating proposal status for ID: {Id} to {Status}", proposalId, status);
            return await _customerService.UpdateProposalStatusAsync(proposalId, status, cancellationToken);
        }

        public async Task<int> SelectProposalAndCreateOrderAsync(int proposalId, int customerId, CancellationToken cancellationToken)
        {
            _logger.Information("CustomerAppService: Processing SelectProposal for ProposalId: {ProposalId}, CustomerId: {CustomerId}", proposalId, customerId);

            var proposal = await _customerService.GetProposalByIdAsync(proposalId, cancellationToken);
            if (proposal == null)
            {
                _logger.Warning("CustomerAppService: Proposal not found for ProposalId: {ProposalId}", proposalId);
                throw new InvalidOperationException("پیشنهاد یافت نشد.");
            }

            var fullProposal = await _customerService.GetFullProposalByIdAsync(proposalId, cancellationToken);
            if (fullProposal?.Request?.CustomerId != customerId)
            {
                _logger.Warning("CustomerAppService: Proposal not authorized for ProposalId: {ProposalId}, CustomerId: {CustomerId}", proposalId, customerId);
                throw new UnauthorizedAccessException("شما دسترسی به این پیشنهاد ندارید.");
            }

            if (proposal.Status != ProposalStatus.Accepted)
            {
                _logger.Warning("CustomerAppService: Proposal is not in Accepted state for ProposalId: {ProposalId}", proposalId);
                throw new InvalidOperationException("این پیشنهاد قابل انتخاب نیست.");
            }

            var createOrderDto = new CreateOrderDto
            {
                CustomerId = customerId,
                ExpertId = proposal.ExpertId,
                RequestId = proposal.RequestId,
                ProposalId = proposal.Id,
                FinalPrice = proposal.Price,
                PaymentStatus = PaymentStatus.Pending
            };

            var orderCreated = await _orderAppService.CreateAsync(createOrderDto, cancellationToken);
            if (!orderCreated)
            {
                _logger.Error("CustomerAppService: Failed to create order from ProposalId: {ProposalId}", proposalId);
                throw new InvalidOperationException("ایجاد سفارش با خطا مواجه شد.");
            }

            var order = await _orderAppService.GetByProposalIdAsync(proposalId, cancellationToken);
            if (order == null)
            {
                _logger.Error("CustomerAppService: Newly created order not found for ProposalId: {ProposalId}", proposalId);
                throw new InvalidOperationException("سفارش ایجاد شده یافت نشد.");
            }

            var proposalToUpdate = new Proposal
            {
                Id = proposal.Id,
                OrderId = order.Id,
                Status = proposal.Status,
                IsEnabled = proposal.IsEnabled
            };
            await _customerService.UpdateProposalAsync(proposalToUpdate, cancellationToken);

            _logger.Information("CustomerAppService: Order created successfully for ProposalId: {ProposalId}, OrderId: {OrderId}", proposalId, order.Id);
            return order.Id;
        }

        public async Task<RequestDto> GetRequestByIdAsync(int requestId, CancellationToken cancellationToken)
        {
            _logger.Information("CustomerAppService: Fetching request by ID: {Id}", requestId);
            return await _customerService.GetRequestByIdAsync(requestId, cancellationToken);
        }
        public async Task<decimal> GetBalanceAsync(int customerId, CancellationToken cancellationToken)
        {
            return await _customerService.GetBalanceAsync(customerId, cancellationToken);
        }

        public async Task<bool> UpdateBalanceAsync(int customerId, decimal newBalance, CancellationToken cancellationToken)
        {
            return await _customerService.UpdateBalanceAsync(customerId, newBalance, cancellationToken);
        }
    }
}
