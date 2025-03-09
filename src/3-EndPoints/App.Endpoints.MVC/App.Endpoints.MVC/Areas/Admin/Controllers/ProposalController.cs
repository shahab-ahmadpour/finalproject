using App.Domain.Core.DTO.Proposals;
using App.Domain.Core.Services.Interfaces.IAppService;
using App.Domain.Core.Services.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProposalController : Controller
    {

        private readonly IProposalAppService _proposalAppService;
        private readonly Serilog.ILogger _logger;

        public ProposalController(IProposalAppService proposalAppService, Serilog.ILogger logger)
        {
            _proposalAppService = proposalAppService;
            _logger = logger;

        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            _logger.Information("Loading all proposals.");

            var proposals = await _proposalAppService.GetAllAsync(cancellationToken);

            if (proposals == null || !proposals.Any())
            {
                _logger.Warning("No proposals found.");
                return View(new List<ProposalDto>());
            }

            _logger.Information("Loaded {Count} proposals.", proposals.Count);
            return View(proposals);
        }

        [HttpGet]
        public async Task<IActionResult> Proposals(int orderId, CancellationToken cancellationToken)
        {
            _logger.Information("Loading proposals for order ID: {OrderId}", orderId);

            var proposals = await _proposalAppService.GetProposalsByOrderIdAsync(orderId, cancellationToken);

            if (proposals == null || !proposals.Any())
            {
                _logger.Warning("No proposals found for order ID: {OrderId}", orderId);
                return View(new List<ProposalDto>());
            }

            _logger.Information("Loaded {Count} proposals for order ID: {OrderId}", proposals.Count, orderId);
            return View(proposals);
        }

    }
}
