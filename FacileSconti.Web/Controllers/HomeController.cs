using FacileSconti.Application.Interfaces;
using FacileSconti.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICouponService _couponService;

    public HomeController(ICouponService couponService) => _couponService = couponService;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var coupons = await _couponService.GetPublicCouponsAsync(null, null, 1, 12, cancellationToken);
        return View(new HomeViewModel
        {
            FeaturedCoupons = coupons.Where(c => !c.IsBoostedInHome).Take(6).ToList(),
            BoostedCoupons = coupons.Where(c => c.IsBoostedInHome).Take(6).ToList()
        });
    }

    [Route("error/404")]
    public IActionResult Error404() => View();

    [Route("error/500")]
    public IActionResult Error500() => View();
}
