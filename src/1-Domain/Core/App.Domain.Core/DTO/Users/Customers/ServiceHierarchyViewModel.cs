using App.Domain.Core.DTO.Categories;
using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.DTO.SubHomeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.Customers
{
    public class ServiceHierarchyViewModel
    {
        public List<CategoryDto> Categories { get; set; }
        public Dictionary<int, List<HomeServiceDto>> HomeServicesByCategory { get; set; }
        public Dictionary<int, List<SubHomeServiceListItemDto>> SubHomeServicesByHomeService { get; set; }
    }
}
