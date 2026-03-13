using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class CouponPaymentConfig : AuditableEntity
{
    public int Id { get; set; }
    public int CouponId { get; set; }
    public decimal? PlatformFeePercentage { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsOnlinePaymentEnabled { get; set; }

    public Coupon Coupon { get; set; } = default!;
}
