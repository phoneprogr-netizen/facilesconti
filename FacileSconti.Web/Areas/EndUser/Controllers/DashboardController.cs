using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Areas.EndUser.Controllers;

[Area("EndUser")]
[Authorize(Roles = "EndUser")]
public class DashboardController : Controller
{
    public IActionResult Index() => View();
}
