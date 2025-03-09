using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Enums
{
    public enum PaymentStatus
    {
        [Display(Name = "در انتظار")]
        Pending,
        [Display(Name = "پرداخت‌شده")]
        paid,
        [Display(Name = "ناموفق")]
        Failed,
        [Display(Name = "انجام شده")]
        Completed,
    }
}
