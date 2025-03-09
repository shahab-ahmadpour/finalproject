using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.HomeServices
{
    public class CreateHomeServiceDto
    {
        [Required(ErrorMessage = "نام سرویس اجباری است")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "توضیحات اجباری است")]
        public string Description { get; set; } = null!;

        public string? ImagePath { get; set; } = null!;

        [Required(ErrorMessage = "دسته‌بندی اجباری است")]
        public int CategoryId { get; set; }
    }
}
