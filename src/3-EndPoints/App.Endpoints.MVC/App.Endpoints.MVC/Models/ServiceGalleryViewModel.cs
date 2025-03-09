using App.Domain.Core.DTO.Categories;
using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.DTO.SubHomeServices;

namespace App.Endpoints.MVC.Models
{
    public class ServiceGalleryViewModel
    {
        public List<CategoryDto> Categories { get; set; }
        public Dictionary<int, List<App.Domain.Core.Services.Entities.HomeService>> HomeServicesByCategory { get; set; }
        public Dictionary<int, List<SubHomeServiceListItemDto>> SubHomeServicesByHomeService { get; set; }
    }
}
