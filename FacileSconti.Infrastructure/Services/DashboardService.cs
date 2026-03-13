using FacileSconti.Application.DTOs;
using FacileSconti.Application.Interfaces;
using FacileSconti.Domain.Enums;
using FacileSconti.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FacileSconti.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _db;
    public DashboardService(ApplicationDbContext db) => _db = db;

    public async Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default) => new()
    {
        ActiveCustomers = await _db.CustomerBusinesses.CountAsync(x => x.IsActive, cancellationToken),
        ActiveContracts = await _db.CustomerContracts.CountAsync(x => x.Status == ContractStatus.Active, cancellationToken),
        ExpiredContracts = await _db.CustomerContracts.CountAsync(x => x.Status == ContractStatus.Expired, cancellationToken),
        ActiveCoupons = await _db.Coupons.CountAsync(x => x.Status == CouponStatus.Active, cancellationToken),
        ExpiredCoupons = await _db.Coupons.CountAsync(x => x.Status == CouponStatus.Expired, cancellationToken),
        TotalDownloads = await _db.CouponDownloads.CountAsync(cancellationToken)
    };

    public async Task<CustomerDashboardDto?> GetCustomerDashboardAsync(string ownerUserId, CancellationToken cancellationToken = default)
    {
        var business = await _db.CustomerBusinesses.Include(x => x.Contracts).ThenInclude(c => c.SubscriptionPlan)
            .Include(x => x.Coupons).FirstOrDefaultAsync(x => x.OwnerUserId == ownerUserId, cancellationToken);
        if (business is null) return null;
        var contract = business.Contracts.FirstOrDefault(c => c.Status == ContractStatus.Active);
        if (contract is null) return null;

        return new CustomerDashboardDto
        {
            BusinessName = business.Name,
            PlanName = contract.SubscriptionPlan.Name,
            DaysToExpiration = contract.EndDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber,
            ActiveCoupons = business.Coupons.Count(c => c.Status == CouponStatus.Active),
            MonthlyDownloads = await _db.CouponDownloads.CountAsync(d => d.Coupon.CustomerBusinessId == business.Id && d.DownloadedAt >= DateTime.UtcNow.AddDays(-30), cancellationToken)
        };
    }
}
