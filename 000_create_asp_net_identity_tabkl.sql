-- ============================================
-- ASP.NET Core Identity - Schema base standard
-- SQL Server
-- ============================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- =========================
-- AspNetRoles
-- =========================
IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AspNetRoles
    (
        Id NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NULL,
        NormalizedName NVARCHAR(256) NULL,
        ConcurrencyStamp NVARCHAR(MAX) NULL,
        CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
    );
END
GO

-- =========================
-- AspNetUsers
-- =========================
IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AspNetUsers
    (
        Id NVARCHAR(450) NOT NULL,
        UserName NVARCHAR(256) NULL,
        NormalizedUserName NVARCHAR(256) NULL,
        Email NVARCHAR(256) NULL,
        NormalizedEmail NVARCHAR(256) NULL,
        EmailConfirmed BIT NOT NULL CONSTRAINT DF_AspNetUsers_EmailConfirmed DEFAULT(0),
        PasswordHash NVARCHAR(MAX) NULL,
        SecurityStamp NVARCHAR(MAX) NULL,
        ConcurrencyStamp NVARCHAR(MAX) NULL,
        PhoneNumber NVARCHAR(MAX) NULL,
        PhoneNumberConfirmed BIT NOT NULL CONSTRAINT DF_AspNetUsers_PhoneNumberConfirmed DEFAULT(0),
        TwoFactorEnabled BIT NOT NULL CONSTRAINT DF_AspNetUsers_TwoFactorEnabled DEFAULT(0),
        LockoutEnd DATETIMEOFFSET(7) NULL,
        LockoutEnabled BIT NOT NULL CONSTRAINT DF_AspNetUsers_LockoutEnabled DEFAULT(0),
        AccessFailedCount INT NOT NULL CONSTRAINT DF_AspNetUsers_AccessFailedCount DEFAULT(0),
        CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id)
    );
END
GO

-- =========================
-- AspNetRoleClaims
-- =========================
IF OBJECT_ID(N'dbo.AspNetRoleClaims', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AspNetRoleClaims
    (
        Id INT IDENTITY(1,1) NOT NULL,
        RoleId NVARCHAR(450) NOT NULL,
        ClaimType NVARCHAR(MAX) NULL,
        ClaimValue NVARCHAR(MAX) NULL,
        CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
        CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId
            FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id)
            ON DELETE CASCADE
    );
END
GO

-- =========================
-- AspNetUserClaims
-- =========================
IF OBJECT_ID(N'dbo.AspNetUserClaims', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AspNetUserClaims
    (
        Id INT IDENTITY(1,1) NOT NULL,
        UserId NVARCHAR(450) NOT NULL,
        ClaimType NVARCHAR(MAX) NULL,
        ClaimValue NVARCHAR(MAX) NULL,
        CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
        CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id)
            ON DELETE CASCADE
    );
END
GO

-- =========================
-- AspNetUserLogins
-- =========================
IF OBJECT_ID(N'dbo.AspNetUserLogins', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AspNetUserLogins
    (
        LoginProvider NVARCHAR(450) NOT NULL,
        ProviderKey NVARCHAR(450) NOT NULL,
        ProviderDisplayName NVARCHAR(MAX) NULL,
        UserId NVARCHAR(450) NOT NULL,
        CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
        CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id)
            ON DELETE CASCADE
    );
END
GO

-- =========================
-- AspNetUserRoles
-- =========================
IF OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AspNetUserRoles
    (
        UserId NVARCHAR(450) NOT NULL,
        RoleId NVARCHAR(450) NOT NULL,
        CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id)
            ON DELETE CASCADE,
        CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId
            FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id)
            ON DELETE CASCADE
    );
END
GO

-- =========================
-- AspNetUserTokens
-- =========================
IF OBJECT_ID(N'dbo.AspNetUserTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AspNetUserTokens
    (
        UserId NVARCHAR(450) NOT NULL,
        LoginProvider NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(450) NOT NULL,
        [Value] NVARCHAR(MAX) NULL,
        CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, [Name]),
        CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id)
            ON DELETE CASCADE
    );
END
GO

-- =========================
-- Indici standard Identity
-- =========================
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'RoleNameIndex' AND object_id = OBJECT_ID('dbo.AspNetRoles')
)
BEGIN
    CREATE UNIQUE INDEX RoleNameIndex
        ON dbo.AspNetRoles (NormalizedName)
        WHERE NormalizedName IS NOT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'EmailIndex' AND object_id = OBJECT_ID('dbo.AspNetUsers')
)
BEGIN
    CREATE INDEX EmailIndex
        ON dbo.AspNetUsers (NormalizedEmail);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UserNameIndex' AND object_id = OBJECT_ID('dbo.AspNetUsers')
)
BEGIN
    CREATE UNIQUE INDEX UserNameIndex
        ON dbo.AspNetUsers (NormalizedUserName)
        WHERE NormalizedUserName IS NOT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_AspNetRoleClaims_RoleId' AND object_id = OBJECT_ID('dbo.AspNetRoleClaims')
)
BEGIN
    CREATE INDEX IX_AspNetRoleClaims_RoleId
        ON dbo.AspNetRoleClaims (RoleId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_AspNetUserClaims_UserId' AND object_id = OBJECT_ID('dbo.AspNetUserClaims')
)
BEGIN
    CREATE INDEX IX_AspNetUserClaims_UserId
        ON dbo.AspNetUserClaims (UserId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_AspNetUserLogins_UserId' AND object_id = OBJECT_ID('dbo.AspNetUserLogins')
)
BEGIN
    CREATE INDEX IX_AspNetUserLogins_UserId
        ON dbo.AspNetUserLogins (UserId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_AspNetUserRoles_RoleId' AND object_id = OBJECT_ID('dbo.AspNetUserRoles')
)
BEGIN
    CREATE INDEX IX_AspNetUserRoles_RoleId
        ON dbo.AspNetUserRoles (RoleId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_AspNetUserTokens_UserId' AND object_id = OBJECT_ID('dbo.AspNetUserTokens')
)
BEGIN
    CREATE INDEX IX_AspNetUserTokens_UserId
        ON dbo.AspNetUserTokens (UserId);
END
GO