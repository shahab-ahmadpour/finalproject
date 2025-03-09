using App.Domain.Core.DTO.Orders;
using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.DTO.Requests;
using App.Domain.Core.DTO.Reviews;
using System;
using System.Collections.Generic;

namespace App.Domain.Core.DTO.Users.Customers
{
    public class CustomerOrdersViewModel
    {
        public List<RequestDto> Requests { get; set; } = new List<RequestDto>();
        public List<ProposalDto> Proposals { get; set; } = new List<ProposalDto>();
        public List<OrderDto> ActiveOrders { get; set; } = new List<OrderDto>();
        public List<OrderDto> CompletedOrders { get; set; } = new List<OrderDto>();
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }
}