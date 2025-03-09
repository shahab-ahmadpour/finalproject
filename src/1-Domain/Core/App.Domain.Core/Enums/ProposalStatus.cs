using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.Enums
{
    public enum ProposalStatus
    {
        [Display(Name = "در انتظار")]
        Pending,
        [Display(Name = "پذیرفته‌شده")]
        Accepted,
        [Display(Name = "ردشده")]
        Rejected
    }
}
