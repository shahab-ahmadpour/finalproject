using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Services.Entities
{
    public class HomeService
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required, MaxLength(255)]
        public string ImagePath { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public List<SubHomeService> SubHomeServices { get; set; } = new();
    }
}
