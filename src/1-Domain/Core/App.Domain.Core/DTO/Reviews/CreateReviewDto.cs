using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Reviews
{
    public class CreateReviewDto
    {
        public int CustomerId { get; set; }
        public int ExpertId { get; set; }
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

}
