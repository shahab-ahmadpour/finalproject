using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Dashboard
{
    public class DashboardDto
    {
        public int NewOrders { get; set; }
        public int RegisteredUsers { get; set; }
        public int Services { get; set; }
        public int Reviews { get; set; }
    }
}
