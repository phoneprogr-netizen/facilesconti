using FacileSconti.Domain.Common;
using FacileSconti.Domain.Enums;

namespace FacileSconti.Domain.Entities;

public class CustomerBoostActivation : AuditableEntity
{
    public int Id { get; set; }
    public int CouponId { get; set; }
    public int HomeBoostPlanId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public BoostStatus Status { get; set; }

    public Coupon Coupon { get; set; } = default!;
    public HomeBoostPlan HomeBoostPlan { get; set; } = default!;
}
