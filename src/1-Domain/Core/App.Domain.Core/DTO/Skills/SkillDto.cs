﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Skills
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubHomeServiceId { get; set; }
        public string SubHomeServiceName { get; set; }
    }

}
