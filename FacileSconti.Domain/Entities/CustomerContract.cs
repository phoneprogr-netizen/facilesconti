using FacileSconti.Domain.Common;
using FacileSconti.Domain.Enums;

namespace FacileSconti.Domain.Entities;

public class CustomerContract : AuditableEntity
{
    public int Id { get; set; }
    public int CustomerBusinessId { get; set; }
    public int SubscriptionPlanId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ContractStatus Status { get; set; } = ContractStatus.Draft;
    public int MaxActiveCoupons { get; set; }
    public int? MaxDownloadsPerCoupon { get; set; }
    public bool UnlimitedCoupons { get; set; }
    public bool UnlimitedDownloads { get; set; }
    public decimal AgreedPrice { get; set; }
    public PaymentMethod InitialPaymentMethod { get; set; }
    public bool AutoRenewRequested { get; set; }
    public string? AdminNotes { get; set; }

    public CustomerBusiness CustomerBusiness { get; set; } = default!;
    public SubscriptionPlan SubscriptionPlan { get; set; } = default!;
    public ICollection<PaymentRecord> Payments { get; set; } = new List<PaymentRecord>();
}
