using App.Domain.Core.DTO.Users.Experts;
using App.Domain.Core.Users.Entities;
using App.Domain.Core.Users.Interfaces.IRepository;
using App.Infrastructure.Db.SqlServer.Ef;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DbAccess.Repository.Ef.Repositories.Users
{
    public class ExpertRepository : IExpertRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public ExpertRepository(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ExpertDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching Expert with ID: {Id}", id);
            var expert = await _dbContext.Set<Expert>()
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new ExpertDto
                {
                    Id = e.Id,
                    AppUserId = e.AppUserId,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    City = e.City,
                    State = e.State
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (expert == null)
            {
                _logger.Warning("Expert not found for ID: {Id}", id);
            }

            return expert;
        }

        public async Task<List<ExpertDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Fetching all experts");
            return await _dbContext.Set<Expert>()
                .AsNoTracking()
                .Select(e => new ExpertDto
                {
                    Id = e.Id,
                    AppUserId = e.AppUserId,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    City = e.City,
                    State = e.State
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(CreateExpertDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Creating Expert with AppUserId: {AppUserId}", dto.AppUserId);
            var expert = new Expert
            {
                AppUserId = dto.AppUserId,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                State = dto.State
            };

            await _dbContext.Set<Expert>().AddAsync(expert, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateExpertDto dto, CancellationToken cancellationToken)
        {
            _logger.Information("Updating Expert with ID: {Id}", id);
            var expert = await _dbContext.Set<Expert>()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (expert == null)
            {
                _logger.Warning("Expert not found for ID: {Id}", id);
                return false;
            }

            expert.PhoneNumber = dto.PhoneNumber;
            expert.Address = dto.Address;
            expert.City = dto.City;
            expert.State = dto.State;

            _dbContext.Set<Expert>().Update(expert);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.Information("Deleting Expert with ID: {Id}", id);
            var expert = await _dbContext.Set<Expert>()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (expert == null)
            {
                _logger.Warning("Expert not found for ID: {Id}", id);
                return false;
            }

            _dbContext.Set<Expert>().Remove(expert);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<decimal> GetBalanceAsync(int expertId, CancellationToken cancellationToken)
        {
            _logger.Information("Fetching balance for ExpertId: {ExpertId}", expertId);
            var expert = await _dbContext.Experts
                .Include(e => e.AppUser)
                .FirstOrDefaultAsync(e => e.Id == expertId, cancellationToken);

            if (expert == null || expert.AppUser == null)
            {
                _logger.Warning("Expert or AppUser not found for ExpertId: {ExpertId}", expertId);
                throw new KeyNotFoundException($"Expert with ID {expertId} not found.");
            }

            return expert.AppUser.AccountBalance;
        }

        public async Task<bool> UpdateBalanceAsync(int expertId, decimal newBalance, CancellationToken cancellationToken)
        {
            _logger.Information("Updating balance for ExpertId: {ExpertId} to {NewBalance}", expertId, newBalance);
            var expert = await _dbContext.Experts
                .Include(e => e.AppUser)
                .FirstOrDefaultAsync(e => e.Id == expertId, cancellationToken);

            if (expert == null || expert.AppUser == null)
            {
                _logger.Warning("Expert or AppUser not found for ExpertId: {ExpertId}", expertId);
                return false;
            }

            expert.AppUser.AccountBalance = newBalance;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.Information("Balance updated for ExpertId: {ExpertId}", expertId);
            return true;
        }

        public async Task<Expert> GetByAppUserIdAsync(int appUserId, CancellationToken cancellationToken)
        {
            _logger.Information("Repository: Fetching Expert by AppUserId: {AppUserId}", appUserId);
            try
            {
                var expert = await _dbContext.Experts
                    .Include(e => e.AppUser)
                    .FirstOrDefaultAsync(e => e.AppUserId == appUserId, cancellationToken);

                if (expert == null)
                {
                    _logger.Warning("Repository: No Expert found for AppUserId: {AppUserId}", appUserId);
                }
                else
                {
                    _logger.Information("Repository: Found Expert with Id: {ExpertId} for AppUserId: {AppUserId}",
                        expert.Id, appUserId);
                }

                return expert;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Repository: Failed to fetch Expert for AppUserId: {AppUserId}", appUserId);
                throw;
            }
        }
    }
}