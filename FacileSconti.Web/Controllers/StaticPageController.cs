using Microsoft.AspNetCore.Mvc;

namespace FacileSconti.Web.Controllers;

public class StaticPageController : Controller
{
    public IActionResult About() => View();
    public IActionResult ChiSiamo() => View();
    public IActionResult DiventaCliente() => View();
    public IActionResult Contatti() => View();
    public IActionResult Privacy() => View();
    public IActionResult Cookie() => View();
}
