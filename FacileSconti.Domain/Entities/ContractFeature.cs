using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class ContractFeature : AuditableEntity
{
    public int Id { get; set; }
    public int SubscriptionPlanId { get; set; }
    public string FeatureKey { get; set; } = string.Empty;
    public string FeatureValue { get; set; } = string.Empty;

    public SubscriptionPlan SubscriptionPlan { get; set; } = default!;
}
