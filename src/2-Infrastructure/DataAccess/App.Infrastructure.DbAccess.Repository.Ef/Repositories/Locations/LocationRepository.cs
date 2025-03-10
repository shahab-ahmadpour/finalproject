using App.Domain.Core.Locations.Interfaces.IRepository;
using App.Domain.Core.Locations;
using App.Infrastructure.Db.SqlServer.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Microsoft.EntityFrameworkCore;
using App.Domain.Core.DTO.City;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Locations
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public LocationRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Province>> GetAllProvincesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching all provinces");
            try
            {
                return await _dbContext.Provinces.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Repository: Error fetching provinces");
                throw;
            }
        }

        public async Task<List<City>> GetAllCitiesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching all cities");
            try
            {
                return await _dbContext.Cities.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Repository: Error fetching cities");
                throw;
            }
        }


        public async Task<List<CityDto>> GetCitiesByProvinceIdAsync(int provinceId, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching cities for ProvinceId: {ProvinceId}", provinceId);
            var cities = await _dbContext.Cities
                .Where(c => c.ProvinceId == provinceId)
                .Select(c => new CityDto
                {
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);

            _logger.Information("Found {Count} cities for ProvinceId: {ProvinceId}", cities.Count, provinceId);
            return cities;
        }

        public async Task<List<CityDto>> GetCitiesByProvinceNameAsync(string provinceName, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching cities for ProvinceName: {ProvinceName}", provinceName);
            var cities = await _dbContext.Cities
                .Where(c => c.Province.Name == provinceName)
                .Select(c => new CityDto
                {
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);

            _logger.Information("Found {Count} cities for ProvinceName: {ProvinceName}", cities.Count, provinceName);
            return cities;
        }
    }
}
