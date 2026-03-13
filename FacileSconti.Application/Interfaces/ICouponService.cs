using FacileSconti.Application.Common;
using FacileSconti.Application.DTOs;

namespace FacileSconti.Application.Interfaces;

public interface ICouponService
{
    Task<IReadOnlyList<CouponCardDto>> GetPublicCouponsAsync(string? categorySlug, string? city, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<CouponCardDto?> GetPublicCouponBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> DownloadCouponAsync(int couponId, string endUserId, CancellationToken cancellationToken = default);
}
