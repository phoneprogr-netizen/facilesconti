USE FacileSconti;
GO

IF COL_LENGTH('SubscriptionPlans', 'SelectableUntil') IS NULL
BEGIN
    ALTER TABLE SubscriptionPlans ADD SelectableUntil DATE NULL;
END
GO
