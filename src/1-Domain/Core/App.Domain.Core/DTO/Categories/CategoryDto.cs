using App.Domain.Core.DTO.HomeServices;
using App.Domain.Core.DTO.SubHomeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<HomeServiceDto> HomeServices { get; set; } = new List<HomeServiceDto>();

    }
}
