using System.ComponentModel.DataAnnotations;

namespace FacileSconti.Web.Areas.Customer.ViewModels;

public class PlanSelectionItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int MaxActiveCoupons { get; set; }
    public int? MaxDownloadsPerCoupon { get; set; }
    public bool UnlimitedCoupons { get; set; }
    public bool UnlimitedDownloads { get; set; }
    public bool AllowsBoost { get; set; }
    public DateOnly? SelectableUntil { get; set; }
}

public class CustomerRenewalViewModel
{
    public List<PlanSelectionItemViewModel> AvailablePlans { get; set; } = [];
    public RenewalPaymentInputViewModel Input { get; set; } = new();
    public List<PaymentMethodOptionViewModel> PaymentMethods { get; set; } = [];
}

public class CustomerContractViewModel
{
    public string BusinessName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int MaxActiveCoupons { get; set; }
    public int? MaxDownloadsPerCoupon { get; set; }
    public bool UnlimitedCoupons { get; set; }
    public bool UnlimitedDownloads { get; set; }
    public int DaysToExpiration { get; set; }
    public bool CanRenewNow { get; set; }
}

public class RenewalPaymentInputViewModel
{
    [Required(ErrorMessage = "Seleziona un piano.")]
    public int? SubscriptionPlanId { get; set; }

    [Required(ErrorMessage = "Seleziona un metodo di pagamento.")]
    public string PaymentMethodCode { get; set; } = string.Empty;
}

public class PaymentMethodOptionViewModel
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class CustomerBoostPageViewModel
{
    public List<BoostPlanItemViewModel> BoostPlans { get; set; } = [];
    public List<CustomerCouponItemViewModel> Coupons { get; set; } = [];
    public CustomerBoostActivationInput Input { get; set; } = new();
    public bool CustomerCanUseBoost { get; set; }
    public string? BoostDisabledReason { get; set; }
}

public class CustomerCouponItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly ValidTo { get; set; }
}

public class BoostPlanItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
}

public class CustomerBoostActivationInput
{
    [Required]
    public int? CouponId { get; set; }

    [Required]
    public int? HomeBoostPlanId { get; set; }
}
