using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Requests
{
    public class UpdateRequestDto
    {
        public RequestStatus Status { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string EnvironmentImagePath { get; set; }
    }

}
