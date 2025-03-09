using App.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Core.DTO.Proposals
{
    public class CreateProposalDto
    {
        [Required(ErrorMessage = "شناسه کارشناس الزامی است")]
        public int ExpertId { get; set; }

        [Required(ErrorMessage = "شناسه درخواست الزامی است")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "مهارت الزامی است")]
        public int SkillId { get; set; }

        [Required(ErrorMessage = "قیمت پیشنهادی الزامی است")]
        [Range(1, double.MaxValue, ErrorMessage = "قیمت پیشنهادی باید بزرگتر از صفر باشد")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "تاریخ اجرا الزامی است")]
        public DateTime ExecutionDate { get; set; }

        [Required(ErrorMessage = "توضیحات الزامی است")]
        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیشتر از 500 کاراکتر باشد")]
        public string Description { get; set; }

        public ProposalStatus Status { get; set; } = ProposalStatus.Pending;

        public DateTime ResponseTime { get; set; } = DateTime.Now;
    }
}