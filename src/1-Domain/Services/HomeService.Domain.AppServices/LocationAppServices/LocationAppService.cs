using App.Domain.Core.Locations;
using App.Domain.Core.Locations.Interfaces.IAppService;
using App.Domain.Core.Locations.Interfaces.IService;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.AppServices.LocationAppServices
{
    public class LocationAppService : ILocationAppService
    {
        private readonly ILocationService _locationService;
        private readonly ILogger _logger;

        public LocationAppService(ILocationService locationService, ILogger logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<List<Province>> GetAllProvincesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all provinces");
            return await _locationService.GetAllProvincesAsync(cancellationToken);
        }

        public async Task<List<City>> GetAllCitiesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching all cities");
            return await _locationService.GetAllCitiesAsync(cancellationToken);
        }

        public async Task<List<City>> GetCitiesByProvinceIdAsync(int provinceId, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching cities for ProvinceId: {ProvinceId}", provinceId);
            return await _locationService.GetCitiesByProvinceIdAsync(provinceId, cancellationToken);
        }

        public async Task<List<City>> GetCitiesByProvinceNameAsync(string provinceName, CancellationToken cancellationToken)
        {
            _logger.Information("AppService: Fetching cities for ProvinceName: {ProvinceName}", provinceName);
            try
            {
                var provinces = await _locationService.GetAllProvincesAsync(cancellationToken);
                var province = provinces.FirstOrDefault(p => p.Name == provinceName);
                if (province == null)
                {
                    _logger.Warning("AppService: Province not found for Name: {ProvinceName}", provinceName);
                    return new List<City>();
                }
                return await _locationService.GetCitiesByProvinceIdAsync(province.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AppService: Error fetching cities for ProvinceName: {ProvinceName}", provinceName);
                throw;
            }
        }
    }

}