using System.ComponentModel.DataAnnotations;

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
