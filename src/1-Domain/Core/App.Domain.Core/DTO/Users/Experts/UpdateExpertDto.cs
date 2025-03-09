using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.Experts
{
    public class UpdateExpertDto
    {
        [MaxLength(15), Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(50)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? City { get; set; }

        [MaxLength(20)]
        public string? State { get; set; }

    }

}
