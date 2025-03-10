using App.Domain.Core.DTO.City;
using App.Domain.Core.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Locations.Interfaces.IAppService
{
    public interface ILocationAppService
    {
        Task<List<Province>> GetAllProvincesAsync(CancellationToken cancellationToken);
        Task<List<City>> GetAllCitiesAsync(CancellationToken cancellationToken);
        Task<List<CityDto>> GetCitiesByProvinceIdAsync(int provinceId, CancellationToken cancellationToken);
        Task<List<CityDto>> GetCitiesByProvinceNameAsync(string provinceName, CancellationToken cancellationToken);
    }

}
