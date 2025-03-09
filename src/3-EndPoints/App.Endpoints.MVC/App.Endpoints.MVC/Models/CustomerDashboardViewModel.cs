using App.Domain.Core.DTO.Orders;
using App.Domain.Core.DTO.Users.Customers;

namespace App.Endpoints.MVC.Models
{
    public class CustomerDashboardViewModel
    {
        public CustomerDto Customer { get; set; }
        public List<OrderDto> Orders { get; set; }
    }
}
