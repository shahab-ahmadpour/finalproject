using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.SubHomeServices
{
    public class UpdateSubHomeServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int Views { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }

    }
}
