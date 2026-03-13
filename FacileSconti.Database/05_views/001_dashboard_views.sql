USE FacileSconti;
GO
CREATE VIEW vw_AdminDashboardSummary AS
SELECT
    (SELECT COUNT(1) FROM CustomerBusinesses WHERE IsActive = 1 AND IsDeleted = 0) AS ActiveCustomers,
    (SELECT COUNT(1) FROM CustomerContracts WHERE Status = 2 AND IsDeleted = 0) AS ActiveContracts,
    (SELECT COUNT(1) FROM CustomerContracts WHERE Status = 4 AND IsDeleted = 0) AS ExpiredContracts,
    (SELECT COUNT(1) FROM Coupons WHERE Status = 2 AND IsDeleted = 0) AS ActiveCoupons,
    (SELECT COUNT(1) FROM Coupons WHERE Status = 4 AND IsDeleted = 0) AS ExpiredCoupons,
    (SELECT COUNT(1) FROM CouponDownloads WHERE IsDeleted = 0) AS TotalDownloads;
GO
