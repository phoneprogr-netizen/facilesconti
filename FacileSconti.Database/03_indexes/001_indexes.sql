USE FacileSconti;
GO
CREATE UNIQUE INDEX UX_SubscriptionPlans_Code ON SubscriptionPlans(Code);
CREATE UNIQUE INDEX UX_CustomerBusinesses_VatNumber ON CustomerBusinesses(VatNumber);
CREATE UNIQUE INDEX UX_CouponCategories_Slug ON CouponCategories(Slug);
CREATE UNIQUE INDEX UX_Coupons_Slug ON Coupons(Slug);
CREATE UNIQUE INDEX UX_CouponDownloads_UniqueCode ON CouponDownloads(UniqueCode);
CREATE UNIQUE INDEX UX_PublicPages_Slug ON PublicPages(Slug);
CREATE INDEX IX_Coupons_Status_ValidTo ON Coupons(Status, ValidTo);
CREATE INDEX IX_CouponDownloads_CouponId_DownloadedAt ON CouponDownloads(CouponId, DownloadedAt);
