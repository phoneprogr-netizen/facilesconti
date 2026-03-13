using FacileSconti.Application.Common;
using FacileSconti.Application.DTOs;
using FacileSconti.Application.Interfaces;
using FacileSconti.Domain.Entities;
using FacileSconti.Domain.Enums;
using FacileSconti.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FacileSconti.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly ApplicationDbContext _db;
    private readonly IQrCodeService _qrCodeService;

    public CouponService(ApplicationDbContext db, IQrCodeService qrCodeService)
    {
        _db = db;
        _qrCodeService = qrCodeService;
    }

    public async Task<IReadOnlyList<CouponCardDto>> GetPublicCouponsAsync(string? categorySlug, string? city, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Coupons.AsNoTracking()
            .Include(c => c.CouponCategory)
            .Include(c => c.CustomerBusiness)
            .Where(c => c.Status == CouponStatus.Active && c.ValidTo >= DateOnly.FromDateTime(DateTime.UtcNow));

        if (!string.IsNullOrWhiteSpace(categorySlug)) query = query.Where(c => c.CouponCategory.Slug == categorySlug);
        if (!string.IsNullOrWhiteSpace(city)) query = query.Where(c => c.CustomerBusiness.City == city);

        return await query.OrderByDescending(c => c.IsBoostedInHome).ThenByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new CouponCardDto
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                CategoryName = c.CouponCategory.Name,
                BusinessName = c.CustomerBusiness.Name,
                ImagePath = c.Images.Where(i => i.IsPrimary).Select(i => i.FilePath).FirstOrDefault(),
                ValidTo = c.ValidTo,
                DownloadsCount = c.Downloads.Count,
                IsBoostedInHome = c.IsBoostedInHome
            }).ToListAsync(cancellationToken);
    }

    public async Task<CouponCardDto?> GetPublicCouponBySlugAsync(string slug, CancellationToken cancellationToken = default)
        => await GetPublicCouponsAsync(null, null, 1, 1000, cancellationToken).ContinueWith(t => t.Result.FirstOrDefault(c => c.Slug == slug), cancellationToken);

    public async Task<ServiceResult<string>> DownloadCouponAsync(int couponId, string endUserId, CancellationToken cancellationToken = default)
    {
        var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Id == couponId, cancellationToken);
        if (coupon is null || coupon.Status != CouponStatus.Active) return ServiceResult<string>.Fail("Coupon non disponibile.");

        var uniqueCode = $"FS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..24];
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        _db.CouponDownloads.Add(new CouponDownload
        {
            CouponId = couponId,
            EndUserId = endUserId,
            UniqueCode = uniqueCode,
            SecureToken = token,
            CreatedBy = endUserId
        });

        await _db.SaveChangesAsync(cancellationToken);
        return ServiceResult<string>.Ok(_qrCodeService.GenerateSvgDataUri(token), uniqueCode);
    }
}
