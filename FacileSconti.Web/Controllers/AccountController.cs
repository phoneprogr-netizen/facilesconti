using FacileSconti.Domain.Entities;
using FacileSconti.Domain.Enums;
using FacileSconti.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _db = db;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    public IActionResult RegisterEndUser() => View();
    public IActionResult RegisterBusiness() => View();
    public IActionResult ForgotPassword() => View();
    public IActionResult ResetPassword() => View();
    public IActionResult AccessDenied() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe = false, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "Inserisci email e password.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Credenziali non valide.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Credenziali non valide.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

        if (await _userManager.IsInRoleAsync(user, "Admin")) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        if (await _userManager.IsInRoleAsync(user, "Customer")) return RedirectToAction("Index", "Dashboard", new { area = "Customer" });

        return RedirectToAction("Index", "Dashboard", new { area = "EndUser" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterEndUser(string firstName, string lastName, string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            UserType = UserType.EndUser
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View();
        }

        await _userManager.AddToRoleAsync(user, "EndUser");
        await _signInManager.SignInAsync(user, false);
        return RedirectToAction("Index", "Dashboard", new { area = "EndUser" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterBusiness(
        string businessName,
        string vatNumber,
        string fiscalCode,
        string email,
        string phone,
        string address,
        string city,
        string province,
        string firstName,
        string lastName,
        string password)
    {
        var alreadyExists = await _userManager.FindByEmailAsync(email);
        if (alreadyExists is not null)
        {
            ModelState.AddModelError(string.Empty, "Esiste già un utente con questa email.");
            return View();
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            UserType = UserType.Customer
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View();
        }

        await _userManager.AddToRoleAsync(user, "Customer");

        _db.CustomerBusinesses.Add(new CustomerBusiness
        {
            Name = businessName,
            VatNumber = vatNumber,
            FiscalCode = fiscalCode,
            Email = email,
            Phone = phone,
            Address = address,
            City = city,
            Province = province,
            OwnerUserId = user.Id,
            CreatedBy = user.Id
        });

        await _db.SaveChangesAsync();
        await _signInManager.SignInAsync(user, false);

        return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
