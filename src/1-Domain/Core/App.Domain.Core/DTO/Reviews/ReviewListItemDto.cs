using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Reviews
{
    public class ReviewListItemDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ExpertName { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
