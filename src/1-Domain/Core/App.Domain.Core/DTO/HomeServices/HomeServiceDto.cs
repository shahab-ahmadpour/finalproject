using App.Domain.Core.DTO.SubHomeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.HomeServices
{
    public class HomeServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }

        public List<SubHomeServiceDto> SubHomeServices { get; set; } = new List<SubHomeServiceDto>();
    }
}
