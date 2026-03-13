using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ManagementController : Controller
{
    public IActionResult Customers() => View();
    public IActionResult Contracts() => View();
    public IActionResult Plans() => View();
    public IActionResult Coupons() => View();
    public IActionResult Categories() => View();
    public IActionResult BusinessRequests() => View();
    public IActionResult Payments() => View();
    public IActionResult Cms() => View();
    public IActionResult AuditLogs() => View();
}
