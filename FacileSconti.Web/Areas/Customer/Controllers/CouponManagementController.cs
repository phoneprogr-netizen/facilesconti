using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FacileSconti.Domain.Entities;
using FacileSconti.Domain.Enums;
using FacileSconti.Infrastructure.Data;
using FacileSconti.Web.Areas.Customer.ViewModels;
using FacileSconti.Web.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace FacileSconti.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class CouponManagementController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PayPalOptions _payPalOptions;
    private static readonly List<PaymentMethodOptionViewModel> RenewalPaymentMethods =
    [
        new()
        {
            Code = "paypal",
            Name = "PayPal",
            Description = "Pagamento immediato online (attivo ora).",
            IsDefault = true
        },
        new()
        {
            Code = "bank_transfer",
            Name = "Bonifico bancario",
            Description = "Disponibile a breve: invio coordinate e conferma manuale."
        },
        new()
        {
            Code = "paymart",
            Name = "Paymart",
            Description = "Disponibile a breve: integrazione gateway Paymart."
        }
    ];

    public CouponManagementController(
        ApplicationDbContext db,
        IHttpClientFactory httpClientFactory,
        IOptions<PayPalOptions> payPalOptions)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _payPalOptions = payPalOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var model = await _db.CustomerBusinesses
            .AsNoTracking()
            .Where(x => x.OwnerUserId == ownerUserId)
            .Select(x => new CustomerProfileViewModel
            {
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                City = x.City,
                Description = x.Description
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
            return NotFound("Profilo attività non trovato per l'utente corrente.");

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(CustomerProfileViewModel model, CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var customerBusiness = await _db.CustomerBusinesses
            .FirstOrDefaultAsync(x => x.OwnerUserId == ownerUserId, cancellationToken);

        if (customerBusiness is null)
            return NotFound("Profilo attività non trovato per l'utente corrente.");

        if (!ModelState.IsValid)
            return View(model);

        customerBusiness.Name = model.Name.Trim();
        customerBusiness.Email = model.Email.Trim();
        customerBusiness.Phone = model.Phone.Trim();
        customerBusiness.City = model.City.Trim();
        customerBusiness.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        TempData["ProfileSuccess"] = "Profilo aggiornato correttamente.";

        return RedirectToAction(nameof(Profile));
    }
    public async Task<IActionResult> Contract(CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var contract = await _db.CustomerContracts
            .AsNoTracking()
            .Where(x => x.CustomerBusiness.OwnerUserId == ownerUserId
                        && x.Status == ContractStatus.Active
                        && x.StartDate <= today
                        && x.EndDate >= today)
            .OrderByDescending(x => x.EndDate)
            .Select(x => new CustomerContractViewModel
            {
                BusinessName = x.CustomerBusiness.Name,
                PlanName = x.SubscriptionPlan.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                MaxActiveCoupons = x.MaxActiveCoupons,
                MaxDownloadsPerCoupon = x.MaxDownloadsPerCoupon,
                UnlimitedCoupons = x.UnlimitedCoupons,
                UnlimitedDownloads = x.UnlimitedDownloads,
                DaysToExpiration = x.EndDate.DayNumber - today.DayNumber,
                CanRenewNow = x.EndDate.DayNumber - today.DayNumber <= 10
            })
            .FirstOrDefaultAsync(cancellationToken);

        return View(contract);
    }

    public async Task<IActionResult> Renewal(CancellationToken cancellationToken)
    {
        if (!await IsRenewalWindowOpenAsync(cancellationToken))
        {
            TempData["RenewalWarning"] = "Il rinnovo è disponibile solo nei 10 giorni precedenti la scadenza del contratto attivo.";
            return RedirectToAction(nameof(Contract));
        }

        SetPayPalViewData();
        var activeContract = await GetActiveContractAsync(cancellationToken);
        var availablePlans = await LoadRenewalPlansAsync(activeContract?.SubscriptionPlanId, cancellationToken);

        var vm = new CustomerRenewalViewModel
        {
            AvailablePlans = availablePlans,
            PaymentMethods = RenewalPaymentMethods,
            CurrentSubscriptionPlanId = activeContract?.SubscriptionPlanId,
            Input = new RenewalPaymentInputViewModel
            {
                SubscriptionPlanId = activeContract?.SubscriptionPlanId ?? availablePlans.FirstOrDefault()?.Id,
                PaymentMethodCode = RenewalPaymentMethods.First(x => x.IsDefault).Code
            }
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renewal(CustomerRenewalViewModel model, CancellationToken cancellationToken)
    {
        if (!await IsRenewalWindowOpenAsync(cancellationToken))
        {
            TempData["RenewalWarning"] = "Il rinnovo è disponibile solo nei 10 giorni precedenti la scadenza del contratto attivo.";
            return RedirectToAction(nameof(Contract));
        }

        SetPayPalViewData();
        var activeContract = await GetActiveContractAsync(cancellationToken);
        model.CurrentSubscriptionPlanId = activeContract?.SubscriptionPlanId;
        model.AvailablePlans = await LoadRenewalPlansAsync(model.CurrentSubscriptionPlanId, cancellationToken);
        model.PaymentMethods = RenewalPaymentMethods;

        var selectedPlan = model.AvailablePlans.FirstOrDefault(x => x.Id == model.Input.SubscriptionPlanId);
        var selectedPaymentMethod = RenewalPaymentMethods.FirstOrDefault(x => x.Code == model.Input.PaymentMethodCode);

        if (selectedPlan is null)
            ModelState.AddModelError(nameof(model.Input.SubscriptionPlanId), "Piano non valido.");
        if (selectedPaymentMethod is null)
            ModelState.AddModelError(nameof(model.Input.PaymentMethodCode), "Metodo di pagamento non valido.");

        if (!ModelState.IsValid)
            return View(model);

        if (selectedPaymentMethod!.Code != "paypal")
        {
            TempData["RenewalWarning"] = $"Il metodo {selectedPaymentMethod.Name} è previsto ma non ancora attivo. Usa PayPal per completare ora.";
            return View(model);
        }

        TempData["RenewalSuccess"] = $"Richiesta di rinnovo inviata: piano {selectedPlan!.Name} con pagamento {selectedPaymentMethod.Name}.";
        return RedirectToAction(nameof(Contract));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePayPalOrder([FromBody] RenewalPaymentInputViewModel input, CancellationToken cancellationToken)
    {
        if (!_payPalOptions.Enabled)
            return BadRequest(new { message = "PayPal non è abilitato nei settaggi applicativi." });

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var selectedPlan = await _db.SubscriptionPlans
            .AsNoTracking()
            .Where(x => x.Id == input.SubscriptionPlanId && x.IsActive && !x.IsDeleted && (x.SelectableUntil == null || x.SelectableUntil >= today))
            .Select(x => new { x.Id, x.Name, x.BasePrice })
            .FirstOrDefaultAsync(cancellationToken);

        if (selectedPlan is null)
            return BadRequest(new { message = "Piano non valido o non disponibile." });

        var accessToken = await GetPayPalAccessTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(accessToken))
            return StatusCode(502, new { message = "Impossibile autenticarsi con PayPal." });

        var orderRequest = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    description = $"Rinnovo piano {selectedPlan.Name}",
                    amount = new
                    {
                        currency_code = _payPalOptions.CurrencyCode,
                        value = selectedPlan.BasePrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                    }
                }
            }
        };

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(GetPayPalApiBaseUrl());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var createOrderRequest = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders")
        {
            Content = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json")
        };
        createOrderRequest.Headers.Add("Prefer", "return=representation");

        using var createOrderResponse = await client.SendAsync(createOrderRequest, cancellationToken);
        var content = await createOrderResponse.Content.ReadAsStringAsync(cancellationToken);
        if (!createOrderResponse.IsSuccessStatusCode)
            return StatusCode((int)createOrderResponse.StatusCode, new { message = "Creazione ordine PayPal fallita.", details = content });

        using var doc = JsonDocument.Parse(content);
        var orderId = doc.RootElement.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
        if (string.IsNullOrWhiteSpace(orderId))
            return StatusCode(502, new { message = "PayPal ha risposto senza order id." });

        return Ok(new { id = orderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapturePayPalOrder([FromBody] CapturePayPalOrderRequest request, CancellationToken cancellationToken)
    {
        if (!_payPalOptions.Enabled)
            return BadRequest(new { message = "PayPal non è abilitato nei settaggi applicativi." });
        if (string.IsNullOrWhiteSpace(request.OrderId))
            return BadRequest(new { message = "Order id PayPal mancante." });

        var accessToken = await GetPayPalAccessTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(accessToken))
            return StatusCode(502, new { message = "Impossibile autenticarsi con PayPal." });

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(GetPayPalApiBaseUrl());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var captureResponse = await client.PostAsync($"/v2/checkout/orders/{request.OrderId}/capture", content: null, cancellationToken);
        var content = await captureResponse.Content.ReadAsStringAsync(cancellationToken);
        if (!captureResponse.IsSuccessStatusCode)
            return StatusCode((int)captureResponse.StatusCode, new { message = "Cattura pagamento PayPal fallita.", details = content });

        TempData["RenewalSuccess"] = "Pagamento PayPal acquisito correttamente. Il rinnovo è stato registrato.";
        return Ok(new { success = true, redirectUrl = Url.Action(nameof(Contract)) });
    }

    [HttpGet]
    public async Task<IActionResult> Coupons(CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var items = await _db.Coupons
            .AsNoTracking()
            .Where(x => x.CustomerBusiness.OwnerUserId == ownerUserId && !x.IsDeleted)
            .Include(x => x.CouponCategory)
            .OrderByDescending(x => x.ValidTo)
            .Select(x => new CustomerCouponListItemViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Category = x.CouponCategory.Name,
                ValidTo = x.ValidTo,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);

        return View(items);
    }

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

    [HttpGet]
    public async Task<IActionResult> EditCoupon(int id, CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var model = await _db.Coupons
            .AsNoTracking()
            .Where(x => x.Id == id && x.CustomerBusiness.OwnerUserId == ownerUserId && !x.IsDeleted)
            .Select(x => new CustomerEditCouponViewModel
            {
                Id = x.Id,
                Title = x.Title,
                ShortDescription = x.ShortDescription,
                FullDescription = x.FullDescription,
                Status = x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
            return NotFound("Coupon non trovato.");

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCoupon(CustomerEditCouponViewModel model, CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var coupon = await _db.Coupons
            .FirstOrDefaultAsync(x => x.Id == model.Id && x.CustomerBusiness.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (coupon is null)
            return NotFound("Coupon non trovato.");

        if (!ModelState.IsValid)
            return View(model);

        coupon.Title = model.Title.Trim();
        coupon.ShortDescription = model.ShortDescription.Trim();
        coupon.FullDescription = model.FullDescription.Trim();
        coupon.Status = model.Status;

        await _db.SaveChangesAsync(cancellationToken);
        TempData["CouponSuccess"] = $"Coupon '{coupon.Title}' aggiornato correttamente.";

        return RedirectToAction(nameof(Coupons));
    }
    [HttpGet]
    public async Task<IActionResult> Statistics(CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var now = DateTime.UtcNow;
        var start30Days = now.AddDays(-30);

        var activeCoupons = await _db.Coupons
            .AsNoTracking()
            .CountAsync(x => x.CustomerBusiness.OwnerUserId == ownerUserId && x.Status == CouponStatus.Active && !x.IsDeleted, cancellationToken);

        var downloadsLast30Days = await _db.CouponDownloads
            .AsNoTracking()
            .CountAsync(x => x.Coupon.CustomerBusiness.OwnerUserId == ownerUserId && x.DownloadedAt >= start30Days, cancellationToken);

        var redeemedLast30Days = await _db.CouponDownloads
            .AsNoTracking()
            .CountAsync(x => x.Coupon.CustomerBusiness.OwnerUserId == ownerUserId && x.DownloadedAt >= start30Days && x.IsRedeemed, cancellationToken);

        var viewsLast30Days = await _db.CouponViews
            .AsNoTracking()
            .CountAsync(x => x.Coupon.CustomerBusiness.OwnerUserId == ownerUserId && x.ViewedAt >= start30Days, cancellationToken);

        var usageRate = downloadsLast30Days == 0
            ? 0
            : Math.Round((decimal)redeemedLast30Days / downloadsLast30Days * 100, 1);

        var vm = new CustomerStatisticsViewModel
        {
            ActiveCoupons = activeCoupons,
            DownloadsLast30Days = downloadsLast30Days,
            UsageRate = usageRate,
            ViewsLast30Days = viewsLast30Days
        };

        return View(vm);
    }

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

    private async Task<bool> IsRenewalWindowOpenAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var contractEndDate = (await GetActiveContractAsync(cancellationToken))?.EndDate;

        if (!contractEndDate.HasValue)
            return false;

        var daysToExpiration = contractEndDate.Value.DayNumber - today.DayNumber;
        return daysToExpiration is >= 0 and <= 10;
    }

    private async Task<CustomerContract?> GetActiveContractAsync(CancellationToken cancellationToken)
    {
        var ownerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _db.CustomerContracts
            .AsNoTracking()
            .Where(x => x.CustomerBusiness.OwnerUserId == ownerUserId
                        && x.Status == ContractStatus.Active
                        && x.StartDate <= today
                        && x.EndDate >= today)
            .OrderByDescending(x => x.EndDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<List<PlanSelectionItemViewModel>> LoadRenewalPlansAsync(int? currentPlanId, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _db.SubscriptionPlans
            .AsNoTracking()
            .Where(x => x.IsActive
                        && !x.IsDeleted
                        && ((x.SelectableUntil == null || x.SelectableUntil >= today)
                            || (currentPlanId.HasValue && x.Id == currentPlanId.Value)))
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
    }

    private void SetPayPalViewData()
    {
        ViewData["PayPalEnabled"] = _payPalOptions.Enabled;
        ViewData["PayPalClientId"] = _payPalOptions.ClientId;
        ViewData["PayPalCurrencyCode"] = _payPalOptions.CurrencyCode;
        ViewData["PayPalEnvironment"] = _payPalOptions.IsSandbox ? "sandbox" : "production";
    }

    private string GetPayPalApiBaseUrl() =>
        _payPalOptions.IsSandbox ? "https://api-m.sandbox.paypal.com" : "https://api-m.paypal.com";

    private async Task<string?> GetPayPalAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_payPalOptions.ClientId) || string.IsNullOrWhiteSpace(_payPalOptions.ClientSecret))
            return null;

        var basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_payPalOptions.ClientId}:{_payPalOptions.ClientSecret}"));
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(GetPayPalApiBaseUrl());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);

        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            })
        };

        using var tokenResponse = await client.SendAsync(tokenRequest, cancellationToken);
        if (!tokenResponse.IsSuccessStatusCode)
            return null;

        var json = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        using var tokenDoc = JsonDocument.Parse(json);
        return tokenDoc.RootElement.TryGetProperty("access_token", out var tokenProp) ? tokenProp.GetString() : null;
    }

    public sealed class CapturePayPalOrderRequest
    {
        public string OrderId { get; set; } = string.Empty;
    }
}
