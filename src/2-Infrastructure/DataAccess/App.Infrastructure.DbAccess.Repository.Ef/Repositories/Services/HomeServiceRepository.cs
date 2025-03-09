using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.Services.Entities;
using App.Domain.Core.Services.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Services
{
    public class HomeServiceRepository : IHomeServiceRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public HomeServiceRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating new home service: {Name}", dto.Name);
            try
            {
                var homeService = new HomeService
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ImagePath = dto.ImagePath,
                    CategoryId = dto.CategoryId,
                    IsActive = true
                };

                await _dbContext.HomeServices.AddAsync(homeService, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully created home service: {Name}", dto.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create home service: {Name}", dto.Name);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateHomeServiceDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating home service with ID: {Id}", id);
            try
            {
                var homeService = await _dbContext.HomeServices.FirstOrDefaultAsync(hs => hs.Id == id, cancellationToken);
                if (homeService == null)
                {
                    _logger.Warning("Home service with ID: {Id} not found", id);
                    return false;
                }

                homeService.Name = dto.Name;
                homeService.Description = dto.Description;
                homeService.CategoryId = dto.CategoryId;
                homeService.IsActive = dto.IsActive;

                if (!string.IsNullOrEmpty(dto.ImagePath))
                {
                    homeService.ImagePath = dto.ImagePath.StartsWith("/") ? dto.ImagePath : "/" + dto.ImagePath;
                    _logger.Information("ImagePath updated to: {ImagePath}", homeService.ImagePath);
                }
                else
                {
                    _logger.Information("ImagePath is empty, keeping the existing ImagePath: {ImagePath}", homeService.ImagePath);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully updated home service with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update home service with ID: {Id}", id);
                return false;
            }
        }


        public async Task<HomeServiceDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching home service with ID: {Id}", id);
            try
            {
                var homeService = await _dbContext.HomeServices
                    .Where(hs => hs.Id == id)
                    .Select(hs => new HomeServiceDto
                    {
                        Id = hs.Id,
                        Name = hs.Name,
                        Description = hs.Description,
                        ImagePath = hs.ImagePath.Replace("\\", "/"),
                        CategoryId = hs.CategoryId,
                        IsActive = hs.IsActive
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (homeService == null)
                {
                    _logger.Warning("Home service with ID: {Id} not found", id);
                }
                else
                {
                    _logger.Information("Fetched home service with ID: {Id}", id);
                }

                return homeService;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch home service with ID: {Id}", id);
                throw;
            }
        }

        public async Task<List<HomeServiceListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all home services.");
            try
            {
                var homeServices = await _dbContext.HomeServices
                    .Include(hs => hs.Category)
                    .Select(hs => new HomeServiceListItemDto
                    {
                        Id = hs.Id,
                        Name = hs.Name,
                        CategoryName = hs.Category.Name,
                        IsActive = hs.IsActive           
                    })
                    .ToListAsync(cancellationToken);

                _logger.Information("Fetched {Count} home services.", homeServices.Count);
                return homeServices;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all home services.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting (deactivating) home service with ID: {Id}", id);
            try
            {
                var homeService = await _dbContext.HomeServices.FindAsync(id);
                if (homeService == null)
                {
                    _logger.Warning("Home service with ID: {Id} not found for deletion.", id);
                    return false;
                }

                homeService.IsActive = false;
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.Information("Successfully deactivated home service with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deactivate home service with ID: {Id}", id);
                return false;
            }
        }
        public async Task<List<HomeService>> GetAllWithSubServicesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.HomeServices
                .Include(hs => hs.SubHomeServices.Where(ss => ss.IsActive))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<HomeService>> GetAllHomeServicesAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all HomeServices with their Categories.");
            try
            {
                var homeServices = await _dbContext.HomeServices
                    .Include(hs => hs.Category)
                    .ToListAsync(cancellationToken);
                _logger.Information("Fetched {Count} HomeServices with Categories.", homeServices.Count);
                return homeServices;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch all HomeServices with Categories.");
                throw;
            }
        }
    }

}
