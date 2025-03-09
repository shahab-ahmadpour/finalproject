using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.SubHomeServices
{
    public class SubHomeServiceDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
        public int Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HomeServiceId { get; set; }
        public string HomeServiceName { get; set; } = null!;
    }
}
