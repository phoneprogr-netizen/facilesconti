using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class CouponManagementController : Controller
{
    public IActionResult Profile() => View();
    public IActionResult Contract() => View();
    public IActionResult Renewal() => View();
    public IActionResult Coupons() => View();
    public IActionResult CreateCoupon() => View();
    public IActionResult EditCoupon(int id) => View();
    public IActionResult Statistics() => View();
    public IActionResult Boost() => View();
}
