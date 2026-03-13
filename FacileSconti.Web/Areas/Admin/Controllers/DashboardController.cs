using FacileSconti.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;
    public async Task<IActionResult> Index(CancellationToken cancellationToken) => View(await _dashboardService.GetAdminDashboardAsync(cancellationToken));
}
