using FacileSconti.Application.Interfaces;
using FacileSconti.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Controllers;

public class CouponsController : Controller
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService) => _couponService = couponService;

    public async Task<IActionResult> Index(string? category, string? city, CancellationToken cancellationToken)
    {
        var coupons = await _couponService.GetPublicCouponsAsync(category, city, 1, 30, cancellationToken);
        return View(new CouponListViewModel { Category = category, City = city, Coupons = coupons });
    }

    [HttpGet("coupons/{slug}")]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        var coupon = await _couponService.GetPublicCouponBySlugAsync(slug, cancellationToken);
        if (coupon is null) return NotFound();

        var similarCoupons = await _couponService.GetSimilarPublicCouponsAsync(coupon.Id, coupon.CategoryName, 4, cancellationToken);
        return View(new CouponDetailViewModel
        {
            Coupon = coupon,
            SimilarCoupons = similarCoupons
        });
    }

    [Authorize(Roles = "EndUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Download(int couponId, CancellationToken cancellationToken)
    {
        var result = await _couponService.DownloadCouponAsync(couponId, User.FindFirst("sub")?.Value ?? User.Identity!.Name!, cancellationToken);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? $"Coupon scaricato. Codice: {result.Message}" : result.Message;
        return RedirectToAction(nameof(Index));
    }
}
