using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class CouponDownload : AuditableEntity
{
    public long Id { get; set; }
    public int CouponId { get; set; }
    public string EndUserId { get; set; } = string.Empty;
    public string UniqueCode { get; set; } = string.Empty;
    public string SecureToken { get; set; } = string.Empty;
    public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
    public bool IsRedeemed { get; set; }

    public Coupon Coupon { get; set; } = default!;
    public ApplicationUser EndUser { get; set; } = default!;
}
