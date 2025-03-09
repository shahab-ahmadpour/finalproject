using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Categories
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public bool? IsActive { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
