namespace FacileSconti.Application.DTOs;

public class AdminDashboardDto
{
    public int ActiveCustomers { get; set; }
    public int ActiveContracts { get; set; }
    public int ExpiredContracts { get; set; }
    public int ActiveCoupons { get; set; }
    public int ExpiredCoupons { get; set; }
    public int TotalDownloads { get; set; }
}

public class CustomerDashboardDto
{
    public string BusinessName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public int DaysToExpiration { get; set; }
    public int ActiveCoupons { get; set; }
    public int MonthlyDownloads { get; set; }
}
