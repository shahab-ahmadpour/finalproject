using App.Domain.Core.DTO.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Interfaces.IAppService
{
    public interface IDashboardAppService
    {
        Task<DashboardDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
    }
}
