-- Phase 8: AI Recommendation & Personalization Schema Expansion
USE NamelyzeEcomDb;
GO

-- 1. User Activity Tracking Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserActivities')
BEGIN
    CREATE TABLE UserActivities (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
        ProductId INT FOREIGN KEY REFERENCES Products(Id),
        CategoryId INT,
        ActivityType NVARCHAR(50) NOT NULL, -- View, AddToCart, Purchase
        Count INT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
END
GO

-- 2. Ensure UserAddresses exists (Requirement for User Profile)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserAddresses')
BEGIN
    CREATE TABLE UserAddresses (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
        Label NVARCHAR(50) NOT NULL, -- Home, Office, etc.
        AddressLine1 NVARCHAR(MAX) NOT NULL,
        AddressLine2 NVARCHAR(MAX),
        City NVARCHAR(100) NOT NULL,
        District NVARCHAR(100) NOT NULL,
        Area NVARCHAR(100) NOT NULL,
        IsDefault BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
END
GO

-- 3. Ensure LoyaltyPoints exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoyaltyPoints')
BEGIN
    CREATE TABLE LoyaltyPoints (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
        Points INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
END
GO
