using System.ComponentModel.DataAnnotations;
using FacileSconti.Domain.Entities;
using FacileSconti.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FacileSconti.Web.Areas.Admin.ViewModels;

public class AdminCustomerFormViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string VatNumber { get; set; } = string.Empty;

    [Required]
    public string FiscalCode { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Province { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Display(Name = "Logo cliente")]
    public IFormFile? LogoFile { get; set; }

    public string? ExistingLogoPath { get; set; }

    [Display(Name = "Utente proprietario")]
    public string? OwnerUserId { get; set; }

    public List<SelectListItem> AvailableOwnerUsers { get; set; } = [];
}

public class AdminContractFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public int CustomerBusinessId { get; set; }

    [Required]
    public int SubscriptionPlanId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required]
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12));

    [Required]
    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    [Range(0, int.MaxValue)]
    public int MaxActiveCoupons { get; set; }

    [Range(0, int.MaxValue)]
    public int? MaxDownloadsPerCoupon { get; set; }

    public bool UnlimitedCoupons { get; set; }
    public bool UnlimitedDownloads { get; set; }

    [Range(0, 9999999)]
    public decimal AgreedPrice { get; set; }

    public PaymentMethod InitialPaymentMethod { get; set; } = PaymentMethod.ManualBankTransfer;
    public bool AutoRenewRequested { get; set; }
    public string? AdminNotes { get; set; }

    public List<SelectListItem> AvailableCustomers { get; set; } = [];
    public List<SelectListItem> AvailablePlans { get; set; } = [];
}

public class AdminCouponFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required]
    public int CustomerBusinessId { get; set; }

    [Required]
    public int CouponCategoryId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required, StringLength(400)]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    public string FullDescription { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal? OriginalPrice { get; set; }

    [Range(0, 999999)]
    public decimal? DiscountedPrice { get; set; }

    public CouponType CouponType { get; set; } = CouponType.Free;
    public CouponStatus Status { get; set; } = CouponStatus.Draft;
    public DateOnly ValidFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly ValidTo { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));
    public int? MaxDownloads { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBoostedInHome { get; set; }

    [Display(Name = "Immagini coupon (1-5)")]
    public List<IFormFile> ImageFiles { get; set; } = [];

    public List<AdminCouponImageItemViewModel> ExistingImages { get; set; } = [];

    public List<int> RemoveImageIds { get; set; } = [];

    public List<SelectListItem> AvailableCustomers { get; set; } = [];
    public List<SelectListItem> AvailableCategories { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var existingCount = ExistingImages.Count(x => !RemoveImageIds.Contains(x.Id));
        var newCount = ImageFiles.Count;
        var total = existingCount + newCount;

        if (total < 1)
            yield return new ValidationResult("Inserisci almeno un'immagine per il coupon.", [nameof(ImageFiles)]);

        if (total > 5)
            yield return new ValidationResult("È possibile caricare al massimo 5 immagini per coupon.", [nameof(ImageFiles)]);
    }
}

public class AdminCouponImageItemViewModel
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class AdminStatisticsViewModel
{
    public int TotalCustomers { get; set; }
    public int TotalContracts { get; set; }
    public int ActiveContracts { get; set; }
    public int TotalCoupons { get; set; }
    public int ActiveCoupons { get; set; }
    public int TotalPayments { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }

    public IReadOnlyList<CustomerStatsItem> CustomerBreakdown { get; set; } = [];
}

public class CustomerStatsItem
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int Contracts { get; set; }
    public int ActiveContracts { get; set; }
    public int Coupons { get; set; }
    public int ActiveCoupons { get; set; }
    public decimal TotalPaid { get; set; }
}

public class AdminPlansViewModel
{
    public List<SubscriptionPlan> Plans { get; set; } = [];
    public AdminPlanFormViewModel Form { get; set; } = new();
}

public class AdminPlanFormViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal BasePrice { get; set; }

    [Range(0, int.MaxValue)]
    public int MaxActiveCoupons { get; set; }

    [Range(0, int.MaxValue)]
    public int? MaxDownloadsPerCoupon { get; set; }

    public bool UnlimitedCoupons { get; set; }
    public bool UnlimitedDownloads { get; set; }
    public bool AllowsBoost { get; set; }
    public bool IsActive { get; set; } = true;

    [Display(Name = "Selezionabile fino al")]
    public DateOnly? SelectableUntil { get; set; }
}
