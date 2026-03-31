using FacileSconti.Application.DTOs;

namespace FacileSconti.Web.ViewModels;

public class CouponDetailViewModel
{
    public CouponCardDto Coupon { get; set; } = new();
    public IReadOnlyList<CouponCardDto> SimilarCoupons { get; set; } = Array.Empty<CouponCardDto>();
}
