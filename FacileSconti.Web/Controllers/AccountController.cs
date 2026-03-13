using FacileSconti.Domain.Entities;
using FacileSconti.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Login() => View();
    public IActionResult RegisterEndUser() => View();
    public IActionResult RegisterBusiness() => View();
    public IActionResult ForgotPassword() => View();
    public IActionResult ResetPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterEndUser(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email, UserType = UserType.EndUser };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return View();
        await _userManager.AddToRoleAsync(user, "EndUser");
        await _signInManager.SignInAsync(user, false);
        return RedirectToAction("Index", "Home");
    }
}
