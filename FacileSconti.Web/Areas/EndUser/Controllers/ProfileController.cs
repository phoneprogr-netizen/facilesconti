using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Areas.EndUser.Controllers;

[Area("EndUser")]
[Authorize(Roles = "EndUser")]
public class ProfileController : Controller
{
    public IActionResult Profile() => View();
    public IActionResult MyCoupons() => View();
    public IActionResult Newsletter() => View();
}
