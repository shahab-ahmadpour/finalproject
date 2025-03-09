using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Requests
{
    public class RequestListItemDto
    {
        public int Id { get; set; }
        public string SubHomeServiceName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
