using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class CouponRedemption : AuditableEntity
{
    public long Id { get; set; }
    public long CouponDownloadId { get; set; }
    public DateTime RedeemedAt { get; set; }
    public string? RedeemedByOperator { get; set; }
    public string? Notes { get; set; }
}
