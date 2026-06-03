-- NamelyzeEcom RBAC and Customer Expansion
-- Adds Dynamic Menus, Loyalty Points, and Multiple Addresses

USE NamelyzeEcomDb;
GO

-- 1. Dynamic Menu Management
CREATE TABLE Menus (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Url NVARCHAR(255) NOT NULL,
    Icon NVARCHAR(50),
    ParentMenuId INT FOREIGN KEY REFERENCES Menus(Id),
    SortOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE MenuRolePermissions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MenuId INT NOT NULL FOREIGN KEY REFERENCES Menus(Id),
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    CanView BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- 2. Customer Profile Expansion
CREATE TABLE UserAddresses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Label NVARCHAR(50) NOT NULL, -- e.g. Home, Office, Default
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

CREATE TABLE LoyaltyPoints (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(Id),
    Points INT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- 3. Seed New Roles
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'CustomerSupport')
    INSERT INTO Roles (Name, Description) VALUES ('CustomerSupport', 'Handle customer inquiries and support tickets');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'InventoryManager')
    INSERT INTO Roles (Name, Description) VALUES ('InventoryManager', 'Manage products, stock, and warehouse');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'DeliveryMan')
    INSERT INTO Roles (Name, Description) VALUES ('DeliveryMan', 'Handle order pickups and deliveries');

-- 4. Initial Menu Seed
INSERT INTO Menus (Title, Url, Icon, SortOrder) VALUES 
('Dashboard', '/Admin/Index', 'bi-speedometer2', 1),
('Order Tracking', '/Admin/OrderTracking', 'bi-truck', 2),
('Categories', '/Admin/Categories', 'bi-tags', 3),
('Products', '/Admin/Products', 'bi-box-seam', 4),
('Orders', '/Admin/Orders', 'bi-cart-check', 5),

('Inventory', '/Admin/Inventory', 'bi-building-up', 5),
('Customers', '/Admin/Customers', 'bi-people', 6),
('Coupons', '/Admin/Coupons', 'bi-ticket-perforated', 7),
('Reviews', '/Admin/Reviews', 'bi-chat-left-text', 8),
('Settings', '/Admin/Settings', 'bi-gear', 9);

-- 5. Assign Menus to Super Admin (RoleId=1) and Admin (RoleId=2)
INSERT INTO MenuRolePermissions (MenuId, RoleId)
SELECT Id, 1 FROM Menus;

INSERT INTO MenuRolePermissions (MenuId, RoleId)
SELECT Id, 2 FROM Menus;

GO
