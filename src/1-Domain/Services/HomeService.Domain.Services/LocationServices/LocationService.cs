using App.Domain.Core.Locations.Interfaces.IRepository;
using App.Domain.Core.Locations.Interfaces.IService;
using App.Domain.Core.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using App.Domain.Core.DTO.City;

namespace HomeService.Domain.Services.LocationServices
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger _logger;

        public LocationService(ILocationRepository locationRepository, ILogger logger)
        {
            _locationRepository = locationRepository;
            _logger = logger;
        }

        public async Task<List<Province>> GetAllProvincesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all provinces");
            return await _locationRepository.GetAllProvincesAsync(cancellationToken);
        }

        public async Task<List<City>> GetAllCitiesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching all cities");
            return await _locationRepository.GetAllCitiesAsync(cancellationToken);
        }

        public async Task<List<CityDto>> GetCitiesByProvinceIdAsync(int provinceId, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching cities for ProvinceId: {ProvinceId}", provinceId);
            return await _locationRepository.GetCitiesByProvinceIdAsync(provinceId, cancellationToken);
        }

        public async Task<List<CityDto>> GetCitiesByProvinceNameAsync(string provinceName, CancellationToken cancellationToken)
        {
            _logger.Information("Service: Fetching cities for ProvinceName: {ProvinceName}", provinceName);
            return await _locationRepository.GetCitiesByProvinceNameAsync(provinceName, cancellationToken);
        }
    }
}