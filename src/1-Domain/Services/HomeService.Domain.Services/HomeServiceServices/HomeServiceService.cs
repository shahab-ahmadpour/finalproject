using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Domain.Core.Services.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeService.Domain.Services.HomeServiceServices
{
    public class HomeServiceService : IHomeServiceService
    {
        private readonly IHomeServiceRepository _homeServiceRepository;
        private readonly ILogger _logger;

        public HomeServiceService(IHomeServiceRepository homeServiceRepository, ILogger logger)
        {
            _homeServiceRepository = homeServiceRepository;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Creating HomeService with Name: {Name}", dto.Name);
            return await _homeServiceRepository.CreateAsync(dto, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, UpdateHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Updating HomeService with Id: {Id}", id);
            return await _homeServiceRepository.UpdateAsync(id, dto, cancellationToken);
        }

        public async Task<HomeServiceDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Getting HomeService with Id: {Id}", id);
            return await _homeServiceRepository.GetAsync(id, cancellationToken);
        }

        public async Task<List<HomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Getting all HomeServices");
            return await _homeServiceRepository.GetAllAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Deleting (Deactivating) HomeService with Id: {Id}", id);
            return await _homeServiceRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<List<HomeServiceDto>> GetAllWithSubServicesAsync(CancellationToken cancellationToken)
        {
            var homeServices = await _homeServiceRepository.GetAllWithSubServicesAsync(cancellationToken);
            return homeServices.Select(hs => new HomeServiceDto
            {
                Id = hs.Id,
                Name = hs.Name,
                ImagePath = hs.ImagePath?.Replace("\\", "/") ?? "/images/homeservices/default.jpg",
                SubHomeServices = hs.SubHomeServices.Select(ss => new SubHomeServiceDto
                {
                    Id = ss.Id,
                    Name = ss.Name,
                    Description = ss.Description,
                    Views = ss.Views,
                    BasePrice = ss.BasePrice,
                    HomeServiceId = ss.HomeServiceId,
                    IsActive = ss.IsActive
                }).ToList()
            }).ToList();
        }

        public async Task<List<App.Domain.Core.Services.Entities.HomeService>> GetAllHomeServicesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all HomeServices with their Categories.");
            try
            {
                var homeServices = await _homeServiceRepository.GetAllHomeServicesAsync(cancellationToken);
                _logger.Information("Service: Fetched {Count} HomeServices with Categories.", homeServices?.Count ?? 0);
                return homeServices;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Service: Failed to fetch all HomeServices with Categories.");
                throw;
            }
        }
    }
}