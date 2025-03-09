using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.HomeServices
{
    public class UpdateHomeServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImagePath { get; set; } = null!;
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
