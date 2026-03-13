using FacileSconti.Application.DTOs;

namespace FacileSconti.Application.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
    Task<CustomerDashboardDto?> GetCustomerDashboardAsync(string ownerUserId, CancellationToken cancellationToken = default);
}
