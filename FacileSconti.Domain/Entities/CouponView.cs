namespace FacileSconti.Domain.Entities;

public class CouponView
{
    public long Id { get; set; }
    public int CouponId { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public Coupon Coupon { get; set; } = default!;
}
