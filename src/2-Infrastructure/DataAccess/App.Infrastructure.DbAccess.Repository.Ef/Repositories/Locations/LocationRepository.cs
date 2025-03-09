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

        public async Task<List<City>> GetCitiesByProvinceIdAsync(int provinceId, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching cities for ProvinceId: {ProvinceId}", provinceId);
            try
            {
                var cities = await _dbContext.Cities
                    .Where(c => c.ProvinceId == provinceId)
                    .ToListAsync(cancellationToken);
                _logger.Information("Repository: Found {Count} cities for ProvinceId: {ProvinceId}", cities.Count, provinceId);
                return cities;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Repository: Error fetching cities for ProvinceId: {ProvinceId}", provinceId);
                throw;
            }
        }
    }
}
