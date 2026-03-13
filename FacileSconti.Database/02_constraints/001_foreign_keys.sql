USE FacileSconti;
GO
ALTER TABLE CustomerBusinesses ADD CONSTRAINT FK_CustomerBusinesses_AspNetUsers FOREIGN KEY (OwnerUserId) REFERENCES AspNetUsers(Id);
ALTER TABLE CustomerContacts ADD CONSTRAINT FK_CustomerContacts_CustomerBusinesses FOREIGN KEY (CustomerBusinessId) REFERENCES CustomerBusinesses(Id);
ALTER TABLE ContractFeatures ADD CONSTRAINT FK_ContractFeatures_SubscriptionPlans FOREIGN KEY (SubscriptionPlanId) REFERENCES SubscriptionPlans(Id);
ALTER TABLE CustomerContracts ADD CONSTRAINT FK_CustomerContracts_CustomerBusinesses FOREIGN KEY (CustomerBusinessId) REFERENCES CustomerBusinesses(Id);
ALTER TABLE CustomerContracts ADD CONSTRAINT FK_CustomerContracts_SubscriptionPlans FOREIGN KEY (SubscriptionPlanId) REFERENCES SubscriptionPlans(Id);
ALTER TABLE Coupons ADD CONSTRAINT FK_Coupons_CustomerBusinesses FOREIGN KEY (CustomerBusinessId) REFERENCES CustomerBusinesses(Id);
ALTER TABLE Coupons ADD CONSTRAINT FK_Coupons_CouponCategories FOREIGN KEY (CouponCategoryId) REFERENCES CouponCategories(Id);
ALTER TABLE CouponImages ADD CONSTRAINT FK_CouponImages_Coupons FOREIGN KEY (CouponId) REFERENCES Coupons(Id);
ALTER TABLE CouponDownloads ADD CONSTRAINT FK_CouponDownloads_Coupons FOREIGN KEY (CouponId) REFERENCES Coupons(Id);
ALTER TABLE CouponDownloads ADD CONSTRAINT FK_CouponDownloads_AspNetUsers FOREIGN KEY (EndUserId) REFERENCES AspNetUsers(Id);
ALTER TABLE CouponViews ADD CONSTRAINT FK_CouponViews_Coupons FOREIGN KEY (CouponId) REFERENCES Coupons(Id);
ALTER TABLE CouponRedemptions ADD CONSTRAINT FK_CouponRedemptions_CouponDownloads FOREIGN KEY (CouponDownloadId) REFERENCES CouponDownloads(Id);
ALTER TABLE CouponPaymentConfigs ADD CONSTRAINT FK_CouponPaymentConfigs_Coupons FOREIGN KEY (CouponId) REFERENCES Coupons(Id);
ALTER TABLE PaymentRecords ADD CONSTRAINT FK_PaymentRecords_CustomerContracts FOREIGN KEY (CustomerContractId) REFERENCES CustomerContracts(Id);
ALTER TABLE CustomerBoostActivations ADD CONSTRAINT FK_CustomerBoostActivations_Coupons FOREIGN KEY (CouponId) REFERENCES Coupons(Id);
ALTER TABLE CustomerBoostActivations ADD CONSTRAINT FK_CustomerBoostActivations_HomeBoostPlans FOREIGN KEY (HomeBoostPlanId) REFERENCES HomeBoostPlans(Id);
