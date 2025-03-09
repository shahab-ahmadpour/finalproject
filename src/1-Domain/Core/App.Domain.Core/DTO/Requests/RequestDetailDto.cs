using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Requests
{
    public class RequestDetailDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SubHomeServiceId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = null!;
    }
}
