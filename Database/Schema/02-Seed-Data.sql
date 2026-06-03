-- namelyzeEcom Sample Seed Data
USE NamelyzeEcomDb;
GO

-- 1. Seed Categories
INSERT INTO Categories (Name, Slug, IsActive) VALUES 
('Smart Phones', 'smart-phones', 1),
('Laptops', 'laptops', 1),
('Accessories', 'accessories', 1),
('Gadgets', 'gadgets', 1);
GO

-- 2. Seed Brands
INSERT INTO Brands (Name, Slug, IsActive) VALUES 
('Apple', 'apple', 1),
('Samsung', 'samsung', 1),
('HP', 'hp', 1),
('Logitech', 'logitech', 1);
GO

-- 3. Seed Products
INSERT INTO Products (Name, Slug, Description, SKU, PurchasePrice, RegularPrice, SalePrice, CurrentStock, MinimumStock, IsFeatured, CategoryId, BrandId, IsActive)
VALUES 
('iPhone 14 Pro', 'iphone-14-pro', 'Apple iPhone 14 Pro 128GB', 'IP14-PRO-128', 95000, 115000, 110000, 50, 5, 1, 1, 1, 1),
('Samsung Galaxy S23', 'samsung-galaxy-s23', 'Samsung Galaxy S23 Ultra', 'SAM-S23-ULT', 85000, 105000, 98000, 30, 3, 1, 1, 2, 1),
('HP Pavilion Laptop', 'hp-pavilion-laptop', 'HP Pavilion 15-EG2000', 'HP-PAV-15', 55000, 75000, 72000, 20, 2, 0, 2, 3, 1);
GO

-- 4. Seed Product Images
INSERT INTO ProductImages (ProductId, ImageUrl, IsMain) VALUES 
(1, '/uploads/products/iphone14.jpg', 1),
(2, '/uploads/products/s23.jpg', 1),
(3, '/uploads/products/hp-pav.jpg', 1);
GO

-- 5. Note on Admin User
-- The Admin User (admin@namelyze.com) is now automatically created by the application 
-- at startup to ensure the password hash matches the system logic.
GO

-- 6. Seed Sample Coupon
INSERT INTO Coupons (Code, Type, Value, MinimumOrderAmount, StartDate, EndDate, IsActive)
VALUES ('WELCOME200', 1, 200, 1000, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE()), 1);
GO
