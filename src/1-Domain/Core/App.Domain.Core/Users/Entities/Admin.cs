using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Users.Entities
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;

    }
}
