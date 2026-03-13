using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class CouponCategory : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? IconCss { get; set; }

    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
