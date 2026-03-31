using FacileSconti.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacileSconti.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var model = await _dashboardService.GetCustomerDashboardAsync(ownerUserId, cancellationToken);
        return View(model);
    }
}
