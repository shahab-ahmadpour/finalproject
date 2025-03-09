using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Users.Experts
{
    public class CreateExpertDto
    {
        [Required]
        [MaxLength(15), Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Address { get; set; }

        [Required]
        [MaxLength(20)]
        public string City { get; set; }

        [Required]
        [MaxLength(20)]
        public string State { get; set; }

        [Required]
        public int AppUserId { get; set; }
    }

}
