USE FacileSconti;
GO

/*
Seed demo per ambienti locali:
- 1 cliente business demo
- 1 utente finale demo
- coupon collegati al cliente

Nota: per password/login usare il seeder applicativo (IdentitySeeder) oppure impostare un PasswordHash valido.
*/

DECLARE @CustomerRoleId NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetRoles WHERE NormalizedName = 'CUSTOMER');
DECLARE @EndUserRoleId NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetRoles WHERE NormalizedName = 'ENDUSER');

DECLARE @CustomerUserId NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email = 'cliente.demo@facilesconti.local');
IF @CustomerUserId IS NULL
BEGIN
    SET @CustomerUserId = CONVERT(NVARCHAR(450), NEWID());

    INSERT INTO AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount,
        FirstName, LastName, UserType, CreatedAt, IsActive
    )
    VALUES
    (
        @CustomerUserId,
        'cliente.demo@facilesconti.local', 'CLIENTE.DEMO@FACILESCONTI.LOCAL',
        'cliente.demo@facilesconti.local', 'CLIENTE.DEMO@FACILESCONTI.LOCAL', 1,
        NULL, CONVERT(NVARCHAR(36), NEWID()), CONVERT(NVARCHAR(36), NEWID()), '+39061234567', 0,
        0, NULL, 0, 0,
        'Mario', 'Rossi', 2, SYSUTCDATETIME(), 1
    );
END

IF @CustomerRoleId IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @CustomerUserId AND RoleId = @CustomerRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@CustomerUserId, @CustomerRoleId);
END

DECLARE @EndUserId NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email = 'utente.demo@facilesconti.local');
IF @EndUserId IS NULL
BEGIN
    SET @EndUserId = CONVERT(NVARCHAR(450), NEWID());

    INSERT INTO AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount,
        FirstName, LastName, UserType, CreatedAt, IsActive
    )
    VALUES
    (
        @EndUserId,
        'utente.demo@facilesconti.local', 'UTENTE.DEMO@FACILESCONTI.LOCAL',
        'utente.demo@facilesconti.local', 'UTENTE.DEMO@FACILESCONTI.LOCAL', 1,
        NULL, CONVERT(NVARCHAR(36), NEWID()), CONVERT(NVARCHAR(36), NEWID()), '+393331234567', 0,
        0, NULL, 0, 0,
        'Luca', 'Verdi', 3, SYSUTCDATETIME(), 1
    );
END

IF @EndUserRoleId IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @EndUserId AND RoleId = @EndUserRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@EndUserId, @EndUserRoleId);
END

DECLARE @BusinessId INT = (SELECT TOP 1 Id FROM CustomerBusinesses WHERE OwnerUserId = @CustomerUserId);
IF @BusinessId IS NULL
BEGIN
    INSERT INTO CustomerBusinesses
    (
        Name, VatNumber, FiscalCode, Email, Phone, Address, City, Province, Description, OwnerUserId, CreatedBy
    )
    VALUES
    (
        'Pizzeria Bella Napoli', 'IT12345678901', 'BLLNPL80A01H501X', 'cliente.demo@facilesconti.local',
        '+39061234567', 'Via Roma 10', 'Roma', 'RM', 'Pizzeria artigianale con forno a legna.', @CustomerUserId, 'seed'
    );

    SET @BusinessId = SCOPE_IDENTITY();
END

DECLARE @CategoryId INT = (SELECT TOP 1 Id FROM CouponCategories ORDER BY Id);
IF @CategoryId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Coupons WHERE Slug = 'pizza-2x1-serale')
    BEGIN
        INSERT INTO Coupons
        (
            CustomerBusinessId, CouponCategoryId, Title, Slug, ShortDescription, FullDescription,
            OriginalPrice, DiscountedPrice, CouponType, Status, ValidFrom, ValidTo,
            MaxDownloads, IsFeatured, IsBoostedInHome, CreatedBy
        )
        VALUES
        (
            @BusinessId, @CategoryId, 'Pizza 2x1 serale', 'pizza-2x1-serale',
            'Tutte le sere, seconda pizza omaggio.',
            'Valido da lunedi a giovedi su prenotazione.',
            24.00, 12.00, 1, 2, DATEADD(DAY, -7, CAST(GETUTCDATE() AS DATE)), DATEADD(MONTH, 2, CAST(GETUTCDATE() AS DATE)),
            NULL, 1, 0, 'seed'
        );
    END

    IF NOT EXISTS (SELECT 1 FROM Coupons WHERE Slug = 'menu-pranzo-20')
    BEGIN
        INSERT INTO Coupons
        (
            CustomerBusinessId, CouponCategoryId, Title, Slug, ShortDescription, FullDescription,
            OriginalPrice, DiscountedPrice, CouponType, Status, ValidFrom, ValidTo,
            MaxDownloads, IsFeatured, IsBoostedInHome, CreatedBy
        )
        VALUES
        (
            @BusinessId, @CategoryId, 'Menu pranzo -20%', 'menu-pranzo-20',
            'Sconto immediato sul menu pranzo.',
            'Valido tutti i giorni feriali dalle 12:00 alle 15:00.',
            20.00, 16.00, 1, 2, DATEADD(DAY, -3, CAST(GETUTCDATE() AS DATE)), DATEADD(MONTH, 1, CAST(GETUTCDATE() AS DATE)),
            NULL, 1, 1, 'seed'
        );
    END
END
GO
