using FacileSconti.Web.Areas.Customer.ViewModels;
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
    public IActionResult Boost() => View();
}
