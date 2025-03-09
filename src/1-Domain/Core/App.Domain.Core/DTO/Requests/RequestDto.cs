using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Requests
{
    public class RequestDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SubHomeServiceId { get; set; }
        public string SubHomeServiceName { get; set; }
        public string Description { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime ExecutionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EnvironmentImagePath { get; set; }
        public bool IsEnabled { get; set; }
    }
}