using App.Domain.Core.DTO.SubHomeServices;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Services
{
    public class SubHomeServiceRepository : ISubHomeServiceRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public SubHomeServiceRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(SubHomeService subHomeService, CancellationToken cancellationToken)
        {
            _logger.Information("Creating new SubHomeService with name: {Name}", subHomeService.Name);

            try
            {
                _dbContext.SubHomeServices.Add(subHomeService);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("SubHomeService with name {Name} created successfully.", subHomeService.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating SubHomeService with name: {Name}", subHomeService.Name);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateSubHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating SubHomeService with Id: {Id}", id);

            var subHomeService = await _dbContext.SubHomeServices
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (subHomeService == null)
            {
                _logger.Warning("SubHomeService with Id: {Id} not found.", id);
                return false;
            }

            subHomeService.Name = dto.Name;
            subHomeService.Description = dto.Description;
            subHomeService.Views = dto.Views;
            subHomeService.BasePrice = dto.BasePrice;

            if (!string.IsNullOrEmpty(dto.ImagePath))
            {
                subHomeService.ImagePath = dto.ImagePath;
            }
            else
            {
                _logger.Warning("ImagePath is empty, keeping the existing ImagePath: {ImagePath}", subHomeService.ImagePath);
            }

            subHomeService.IsActive = dto.IsActive;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("SubHomeService with Id: {Id} updated successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating SubHomeService with Id: {Id}.", id);
                return false;
            }
        }

        public async Task<SubHomeServiceDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching SubHomeService with Id: {Id}", id);

            var subHomeService = await _dbContext.SubHomeServices
                .Where(s => s.Id == id)
                .Include(s => s.HomeService)
                .Select(s => new SubHomeServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Views = s.Views,
                    BasePrice = s.BasePrice,
                    ImagePath = s.ImagePath,
                    IsActive = s.IsActive,
                    HomeServiceId = s.HomeServiceId,
                    HomeServiceName = s.HomeService.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (subHomeService == null)
            {
                _logger.Warning("SubHomeService with Id: {Id} not found.", id);
            }

            return subHomeService;
        }

        public async Task<List<SubHomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all SubHomeServices.");

            var subHomeServices = await _dbContext.SubHomeServices
                .Include(s => s.HomeService)
                .Select(s => new SubHomeServiceListItemDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Views = s.Views,
                    Description = s.Description,
                    BasePrice = s.BasePrice,
                    ImagePath = s.ImagePath,
                    IsActive = s.IsActive,
                    HomeServiceName = s.HomeService.Name
                })
                .ToListAsync(cancellationToken);

            return subHomeServices;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Disabling SubHomeService with Id: {Id}", id);

            var subHomeService = await _dbContext.SubHomeServices
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (subHomeService == null)
            {
                _logger.Warning("SubHomeService with Id: {Id} not found.", id);
                return false;
            }

            subHomeService.IsActive = false;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("SubHomeService with Id: {Id} successfully disabled.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error disabling SubHomeService with Id: {Id}.", id);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Checking if SubHomeService with Id: {Id} exists.", id);

            var exists = await _dbContext.SubHomeServices
                .Where(s => s.Id == id)
                .AnyAsync(cancellationToken);

            return exists;
        }

        public async Task<List<SubHomeService>> GetAllServicesAsync(CancellationToken cancellationToken = default)
        {
            _logger.Information("Fetching all SubHomeServices asynchronously.");
            try
            {
                var services = await _dbContext.SubHomeServices.ToListAsync(cancellationToken);
                _logger.Information("Fetched {Count} SubHomeServices.", services?.Count ?? 0);
                return services;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all SubHomeServices asynchronously.");
                throw;
            }
        }

        public async Task<SubHomeServiceListItemDto> GetSubHomeServiceByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching SubHomeService by Id: {Id}", id);
            try
            {
                var subHomeService = await _dbContext.SubHomeServices
                    .Where(s => s.Id == id)
                    .Include(s => s.HomeService)
                    .Select(s => new SubHomeServiceListItemDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Views = s.Views,
                        Description = s.Description,
                        BasePrice = s.BasePrice,
                        ImagePath = s.ImagePath,
                        IsActive = s.IsActive,
                        HomeServiceId = s.HomeServiceId,
                        HomeServiceName = s.HomeService.Name
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (subHomeService == null)
                {
                    _logger.Warning("SubHomeService not found for Id: {Id}", id);
                }
                else
                {
                    _logger.Information("Found SubHomeService with Id: {Id}", id);
                }

                return subHomeService;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch SubHomeService for Id: {Id}", id);
                throw;
            }
        }
        public async Task<List<SubHomeServiceListItemDto>> GetSubHomeServicesByHomeServiceIdAsync(int homeServiceId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching sub-home services for HomeServiceId: {HomeServiceId}", homeServiceId);
            try
            {
                var subHomeServices = await _dbContext.SubHomeServices
                    .Where(s => s.HomeServiceId == homeServiceId)
                    .Select(s => new SubHomeServiceListItemDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description =s.Description,
                        BasePrice = s.BasePrice,
                        Views = s.Views,
                        ImagePath = s.ImagePath
                    })
                    .ToListAsync(cancellationToken);

                if (subHomeServices == null || !subHomeServices.Any())
                {
                    _logger.Warning("No sub-home services found for HomeServiceId: {HomeServiceId}", homeServiceId);
                    return new List<SubHomeServiceListItemDto>();
                }

                _logger.Information("Found {Count} sub-home services for HomeServiceId: {HomeServiceId}", subHomeServices.Count, homeServiceId);
                return subHomeServices;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch sub-home services for HomeServiceId: {HomeServiceId}", homeServiceId);
                throw;
            }
        }
    }

}
