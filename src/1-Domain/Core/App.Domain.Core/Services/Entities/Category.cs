using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Required, MaxLength(255)]
        public string ImagePath { get; set; }
        public bool IsActive { get; set; } = true;

        public List<HomeService> HomeServices { get; set; } = new();
    }
}
