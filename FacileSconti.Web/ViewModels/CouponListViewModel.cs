using FacileSconti.Application.DTOs;

namespace FacileSconti.Web.ViewModels;

public class CouponListViewModel
{
    public string? Category { get; set; }
    public string? City { get; set; }
    public IReadOnlyList<CouponCardDto> Coupons { get; set; } = [];
}
