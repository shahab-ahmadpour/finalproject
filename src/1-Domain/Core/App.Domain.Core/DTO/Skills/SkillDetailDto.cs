using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core._ِDTO.Skills
{
    public class SkillDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SubHomeServiceId { get; set; }
        public string SubHomeServiceName { get; set; } = null!;
    }
}
