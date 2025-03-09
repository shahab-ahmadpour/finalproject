using App.Domain.Core.Services.Interfaces.IAppService;
using App.Endpoints.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace App.Endpoints.MVC.Controllers          

{
    public class HomeController : Controller
    {
        private readonly IHomeServiceAppService _homeServiceAppService;
        private readonly ICategoryAppService _categoryAppService;

        public HomeController(IHomeServiceAppService homeServiceAppService, ICategoryAppService categoryAppService)
        {
            _homeServiceAppService = homeServiceAppService;
            _categoryAppService = categoryAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var homeServices = await _homeServiceAppService.GetAllWithSubServicesAsync(cancellationToken);
            var categories = await _categoryAppService.GetAllWithServicesAsync(cancellationToken);

            ViewData["Categories"] = categories;
            return View(homeServices);
        }
    }
}
