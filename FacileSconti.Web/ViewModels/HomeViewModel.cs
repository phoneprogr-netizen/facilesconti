using FacileSconti.Application.DTOs;

namespace FacileSconti.Web.ViewModels;

public class HomeViewModel
{
    public IReadOnlyList<CouponCardDto> FeaturedCoupons { get; set; } = [];
    public IReadOnlyList<CouponCardDto> BoostedCoupons { get; set; } = [];
}
