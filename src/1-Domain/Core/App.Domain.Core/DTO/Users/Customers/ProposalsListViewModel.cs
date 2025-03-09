using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.DTO.Requests;
using System;
using System.Collections.Generic;

namespace App.Domain.Core.DTO.Users.Customers
{
    public class ProposalsListViewModel
    {
        public RequestDto Request { get; set; }
        public List<ProposalDto> Proposals { get; set; } = new List<ProposalDto>();
        public bool HasActiveOrder { get; set; }
    }
}