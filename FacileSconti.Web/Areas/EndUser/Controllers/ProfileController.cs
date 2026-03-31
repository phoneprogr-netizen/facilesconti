using System.Security.Claims;
using FacileSconti.Domain.Entities;
using FacileSconti.Infrastructure.Data;
using FacileSconti.Web.Areas.EndUser.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacileSconti.Web.Areas.EndUser.Controllers;

[Area("EndUser")]
[Authorize(Roles = "EndUser")]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null) return NotFound();

        var vm = new EndUserProfileViewModel
        {
            FullName = user.UserName ?? "Utente",
            Email = user.Email ?? string.Empty,
            RegisteredAt = DateTime.UtcNow
        };

        return View(vm);
    }

    public async Task<IActionResult> MyCoupons(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var coupons = await _db.CouponDownloads
            .AsNoTracking()
            .Include(x => x.Coupon)
            .Where(x => x.EndUserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.DownloadedAt)
            .Take(100)
            .Select(x => new EndUserCouponItemViewModel
            {
                Title = x.Coupon.Title,
                DownloadedAt = x.DownloadedAt,
                IsRedeemed = x.IsRedeemed,
                Code = x.UniqueCode
            })
            .ToListAsync(cancellationToken);

        return View(new EndUserMyCouponsViewModel { Coupons = coupons });
    }

    public async Task<IActionResult> Newsletter(CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            return View(new EndUserNewsletterViewModel());

        var subscriptions = await _db.NewsletterSubscriptions
            .AsNoTracking()
            .Where(x => x.Email == email && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new NewsletterItemViewModel
            {
                Email = x.Email,
                IsConfirmed = x.IsConfirmed,
                ConfirmedAt = x.ConfirmedAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return View(new EndUserNewsletterViewModel { Subscriptions = subscriptions });
    }
}
