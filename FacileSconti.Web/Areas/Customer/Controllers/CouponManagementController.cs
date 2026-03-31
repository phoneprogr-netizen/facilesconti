using System.Security.Claims;
using FacileSconti.Domain.Entities;
using FacileSconti.Domain.Enums;
using FacileSconti.Infrastructure.Data;
using FacileSconti.Web.Areas.Customer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacileSconti.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class CouponManagementController : Controller
{
    private readonly ApplicationDbContext _db;

    public CouponManagementController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Profile() => View();
    public IActionResult Contract() => View();

    public async Task<IActionResult> Renewal(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var vm = new CustomerRenewalViewModel
        {
            AvailablePlans = await _db.SubscriptionPlans
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
                .ToListAsync(cancellationToken)
        };

        return View(vm);
    }

    public IActionResult Coupons() => View();

    [HttpGet]
    public IActionResult CreateCoupon() => View(new CreateCouponViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateCoupon(CreateCouponViewModel model)
    {
        if (model.ValidFrom.HasValue && model.ValidTo.HasValue && model.ValidTo < model.ValidFrom)
        {
            ModelState.AddModelError(nameof(model.ValidTo), "La data di fine deve essere successiva alla data di inizio");
        }

        if (!ModelState.IsValid)
        {
            TempData["CouponError"] = "Impossibile salvare il coupon: controlla i campi evidenziati.";
            return View(model);
        }

        // TODO: Persistenza coupon su database quando sarà disponibile il flusso completo.
        TempData["CouponSuccess"] = $"Coupon '{model.Title}' salvato correttamente (demo).";
        return RedirectToAction(nameof(CreateCoupon));
    }

    public IActionResult EditCoupon(int id) => View();
    public IActionResult Statistics() => View();

    [HttpGet]
    public async Task<IActionResult> Boost(CancellationToken cancellationToken)
    {
        var vm = await BuildBoostViewModelAsync(new CustomerBoostActivationInput(), cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateBoost(CustomerBoostActivationInput input, CancellationToken cancellationToken)
    {
        var vm = await BuildBoostViewModelAsync(input, cancellationToken);
        if (!vm.CustomerCanUseBoost)
        {
            TempData["BoostError"] = vm.BoostDisabledReason ?? "Il boost non è disponibile per il tuo contratto.";
            return RedirectToAction(nameof(Boost));
        }

        if (!ModelState.IsValid)
            return View("Boost", vm);

        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var customerBusinessId = await _db.CustomerBusinesses
            .Where(x => x.OwnerUserId == ownerUserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var coupon = await _db.Coupons
            .FirstOrDefaultAsync(x => x.Id == input.CouponId && x.CustomerBusinessId == customerBusinessId && x.Status == CouponStatus.Active, cancellationToken);
        var plan = await _db.HomeBoostPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == input.HomeBoostPlanId && x.IsActive && !x.IsDeleted, cancellationToken);

        if (coupon is null || plan is null)
        {
            ModelState.AddModelError(string.Empty, "Selezione non valida. Controlla coupon e piano boost.");
            return View("Boost", vm);
        }

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var activation = new CustomerBoostActivation
        {
            CouponId = coupon.Id,
            HomeBoostPlanId = plan.Id,
            StartDate = startDate,
            EndDate = startDate.AddDays(plan.DurationDays - 1),
            Status = BoostStatus.Requested,
            IsActive = true,
            IsDeleted = false
        };

        _db.CustomerBoostActivations.Add(activation);
        await _db.SaveChangesAsync(cancellationToken);
        TempData["BoostSuccess"] = $"Richiesta inviata: {plan.Name} ({plan.Price:0.00}€).";
        return RedirectToAction(nameof(Boost));
    }

    private async Task<CustomerBoostPageViewModel> BuildBoostViewModelAsync(CustomerBoostActivationInput input, CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var customerBusiness = await _db.CustomerBusinesses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OwnerUserId == ownerUserId, cancellationToken);

        var currentContract = customerBusiness is null
            ? null
            : await _db.CustomerContracts
                .AsNoTracking()
                .Include(x => x.SubscriptionPlan)
                .Where(x => x.CustomerBusinessId == customerBusiness.Id
                            && x.Status == ContractStatus.Active
                            && x.StartDate <= today
                            && x.EndDate >= today)
                .OrderByDescending(x => x.EndDate)
                .FirstOrDefaultAsync(cancellationToken);

        var canUseBoost = currentContract?.SubscriptionPlan.AllowsBoost == true;

        var vm = new CustomerBoostPageViewModel
        {
            Input = input,
            CustomerCanUseBoost = canUseBoost,
            BoostDisabledReason = canUseBoost
                ? null
                : "Il tuo piano attuale non include il servizio boost. Contatta l'admin per un upgrade.",
            BoostPlans = await _db.HomeBoostPlans
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Priority)
                .Select(x => new BoostPlanItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    DurationDays = x.DurationDays,
                    Price = x.Price
                })
                .ToListAsync(cancellationToken),
            Coupons = customerBusiness is null
                ? []
                : await _db.Coupons
                    .AsNoTracking()
                    .Where(x => x.CustomerBusinessId == customerBusiness.Id && x.Status == CouponStatus.Active && !x.IsDeleted)
                    .OrderBy(x => x.Title)
                    .Select(x => new CustomerCouponItemViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ValidTo = x.ValidTo
                    })
                    .ToListAsync(cancellationToken)
        };

        return vm;
    }
}
