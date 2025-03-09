using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Enums
{
    public enum RequestStatus
    {
        [Display(Name = "در انتظار")]
        Pending,
        [Display(Name = "در حال انجام")]
        InProgress,
        [Display(Name = "تکمیل‌شده")]
        Completed,
        [Display(Name = "لغو شده")]
        Cancelled
    }
}
