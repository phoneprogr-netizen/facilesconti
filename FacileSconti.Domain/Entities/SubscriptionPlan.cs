using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class SubscriptionPlan : AuditableEntity
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

    public ICollection<CustomerContract> Contracts { get; set; } = new List<CustomerContract>();
    public ICollection<ContractFeature> Features { get; set; } = new List<ContractFeature>();
}
