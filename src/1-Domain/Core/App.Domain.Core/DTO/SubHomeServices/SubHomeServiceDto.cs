using App.Domain.Core.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.SubHomeServices
{
    public class SubHomeServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public decimal BasePrice { get; set; }
        public int Views { get; set; } = 0;
        public int HomeServiceId { get; set; }
        public bool IsActive { get; set; }
        public string HomeServiceName { get; set; }
    }
}
