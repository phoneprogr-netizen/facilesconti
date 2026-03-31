using System.ComponentModel.DataAnnotations;

namespace FacileSconti.Web.Areas.Customer.ViewModels;

public class CustomerProfileViewModel
{
    [Required(ErrorMessage = "Il nome attività è obbligatorio.")]
    [StringLength(160)]
    [Display(Name = "Nome attività")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email è obbligatoria.")]
    [StringLength(160)]
    [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(40)]
    [Display(Name = "Telefono")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(120)]
    [Display(Name = "Città")]
    public string City { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Descrizione")]
    public string? Description { get; set; }
}

public class CustomerStatisticsViewModel
{
    public int ActiveCoupons { get; set; }
    public int DownloadsLast30Days { get; set; }
    public decimal UsageRate { get; set; }
    public int ViewsLast30Days { get; set; }
}
