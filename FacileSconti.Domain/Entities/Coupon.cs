using FacileSconti.Domain.Common;
using FacileSconti.Domain.Enums;

namespace FacileSconti.Domain.Entities;

public class Coupon : AuditableEntity
{
    public int Id { get; set; }
    public int CustomerBusinessId { get; set; }
    public int CouponCategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string FullDescription { get; set; } = string.Empty;
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public CouponType CouponType { get; set; } = CouponType.Free;
    public CouponStatus Status { get; set; } = CouponStatus.Draft;
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public int? MaxDownloads { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBoostedInHome { get; set; }

    public CustomerBusiness CustomerBusiness { get; set; } = default!;
    public CouponCategory CouponCategory { get; set; } = default!;
    public CouponPaymentConfig? PaymentConfig { get; set; }
    public ICollection<CouponImage> Images { get; set; } = new List<CouponImage>();
    public ICollection<CouponDownload> Downloads { get; set; } = new List<CouponDownload>();
    public ICollection<CouponView> Views { get; set; } = new List<CouponView>();
}
