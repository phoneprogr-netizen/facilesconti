using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class CouponImage : AuditableEntity
{
    public int Id { get; set; }
    public int CouponId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }

    public Coupon Coupon { get; set; } = default!;
}
