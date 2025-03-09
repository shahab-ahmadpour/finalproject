using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.SubHomeServices
{
    public class CreateSubHomeServiceDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Views { get; set; }

        [Required]
        public decimal BasePrice { get; set; }

        [Required]
        public int HomeServiceId { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public IFormFile ImageFile { get; set; } 

        public string ImagePath { get; set; }
    }
}
