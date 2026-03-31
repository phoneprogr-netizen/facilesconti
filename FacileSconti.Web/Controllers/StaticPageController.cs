using FacileSconti.Infrastructure.Data;
using FacileSconti.Web.Areas.Customer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacileSconti.Web.Controllers;

public class StaticPageController : Controller
{
    private readonly ApplicationDbContext _db;

    public StaticPageController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult About() => View();
    public IActionResult ChiSiamo() => View();
    public async Task<IActionResult> DiventaCliente(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var plans = await _db.SubscriptionPlans
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted && (x.SelectableUntil == null || x.SelectableUntil >= today))
            .OrderBy(x => x.BasePrice)
            .Select(x => new PlanSelectionItemViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                BasePrice = x.BasePrice,
                MaxActiveCoupons = x.MaxActiveCoupons,
                MaxDownloadsPerCoupon = x.MaxDownloadsPerCoupon,
                UnlimitedCoupons = x.UnlimitedCoupons,
                UnlimitedDownloads = x.UnlimitedDownloads,
                AllowsBoost = x.AllowsBoost,
                SelectableUntil = x.SelectableUntil
            })
            .ToListAsync(cancellationToken);

        return View(plans);
    }
    public IActionResult Contatti() => View();
    public IActionResult Privacy() => View();
    public IActionResult Cookie() => View();
}
