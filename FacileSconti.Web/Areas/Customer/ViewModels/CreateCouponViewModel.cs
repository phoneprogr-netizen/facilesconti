using System.ComponentModel.DataAnnotations;
using FacileSconti.Domain.Enums;

namespace FacileSconti.Web.Areas.Customer.ViewModels;

public class CreateCouponViewModel
{
    [Required(ErrorMessage = "Il titolo è obbligatorio")]
    [StringLength(120, ErrorMessage = "Il titolo può contenere massimo 120 caratteri")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoria è obbligatoria")]
    [StringLength(60, ErrorMessage = "La categoria può contenere massimo 60 caratteri")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "La data di inizio è obbligatoria")]
    [DataType(DataType.Date)]
    public DateTime? ValidFrom { get; set; }

    [Required(ErrorMessage = "La data di fine è obbligatoria")]
    [DataType(DataType.Date)]
    public DateTime? ValidTo { get; set; }
}

public class CustomerCouponListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateOnly ValidTo { get; set; }
    public CouponStatus Status { get; set; }
}

public class CustomerEditCouponViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Il titolo è obbligatorio")]
    [StringLength(120, ErrorMessage = "Il titolo può contenere massimo 120 caratteri")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descrizione breve è obbligatoria")]
    [StringLength(300, ErrorMessage = "La descrizione breve può contenere massimo 300 caratteri")]
    public string ShortDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descrizione completa è obbligatoria")]
    public string FullDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleziona uno stato")]
    public CouponStatus Status { get; set; }
}
