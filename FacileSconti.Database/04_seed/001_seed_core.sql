USE FacileSconti;
GO
INSERT INTO SubscriptionPlans (Name, Code, BasePrice, MaxActiveCoupons, MaxDownloadsPerCoupon, UnlimitedCoupons, UnlimitedDownloads, AllowsBoost, CreatedBy)
VALUES
('Piano A', 'PLAN-A', 299.00, 1, NULL, 0, 1, 0, 'seed'),
('Piano B', 'PLAN-B', 499.00, 3, NULL, 0, 1, 1, 'seed'),
('Piano C', 'PLAN-C', 599.00, 1, 100, 0, 0, 1, 'seed'),
('Piano D', 'PLAN-D', 999.00, 100, NULL, 1, 1, 1, 'seed'),
('Piano E', 'PLAN-E', 0.00, 0, NULL, 0, 0, 1, 'seed');

INSERT INTO CouponCategories (Name, Slug, IconCss, CreatedBy)
VALUES
('Ristorazione', 'ristorazione', 'bi bi-cup-hot', 'seed'),
('Benessere', 'benessere', 'bi bi-heart-pulse', 'seed'),
('Servizi', 'servizi', 'bi bi-tools', 'seed');

INSERT INTO HomeBoostPlans (Name, DurationDays, Price, Priority, CreatedBy)
VALUES ('Boost 7 giorni', 7, 49.00, 1, 'seed'), ('Boost 30 giorni', 30, 149.00, 2, 'seed');

INSERT INTO PublicPages (Slug, Title, ContentHtml, ShowInFooter, CreatedBy)
VALUES
('chi-siamo', 'Chi siamo', '<p>Contenuto iniziale.</p>', 1, 'seed'),
('privacy-policy', 'Privacy Policy', '<p>Policy iniziale.</p>', 1, 'seed'),
('cookie-policy', 'Cookie Policy', '<p>Cookie policy iniziale.</p>', 1, 'seed');
