# Project Setup & Development Guide

## Quick Start

### Prerequisites
- .NET 9 SDK
- SQL Server 2019 or later
- Visual Studio 2022 or Visual Studio Code

### Database Setup

1. **Create Database** using the SQL script:
```bash
# Using SQL Server Management Studio
# Run: Database/Schema/01-Initial-Schema.sql
```

2. **Update Connection String**
   Edit `src/NamelyzeEcom.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=NamelyzeEcomDb;Trusted_Connection=true;Encrypt=false;TrustServerCertificate=true;"
   }
   ```

### Running the Application

```bash
# Navigate to project directory
cd NamelyzeEcom

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
cd src/NamelyzeEcom.Web
dotnet run

# API will be available at https://localhost:5001
# Swagger documentation at https://localhost:5001/swagger
```

## Project Architecture

### Layered Architecture
```
NamelyzeEcom.Web (Presentation)
    ↓
NamelyzeEcom.Business (Business Logic)
    ↓
NamelyzeEcom.Data (Data Access)
    ↓
NamelyzeEcom.Common (Shared Resources)
```

### Design Patterns Used
1. **Repository Pattern** - Data access abstraction
2. **Unit of Work Pattern** - Transaction management
3. **Dependency Injection** - Loose coupling
4. **DTO Pattern** - Data transfer
5. **Service Layer Pattern** - Business logic isolation

## API Endpoints

### Authentication Endpoints
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `GET /api/auth/profile/{userId}` - Get user profile
- `PUT /api/auth/profile/{userId}` - Update profile
- `POST /api/auth/change-password` - Change password

### Product Endpoints
- `GET /api/products` - Get all products (paginated)
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/slug/{slug}` - Get product by slug
- `GET /api/products/featured` - Get featured products
- `GET /api/products/new-arrivals` - Get new arrivals
- `GET /api/products/search?q=term` - Search products
- `GET /api/products/category/{categoryId}` - Get products by category
- `POST /api/products` - Create product (Admin)
- `PUT /api/products/{id}` - Update product (Admin)
- `DELETE /api/products/{id}` - Delete product (Admin)

### Order Endpoints
- `GET /api/orders/{id}` - Get order by ID
- `GET /api/orders/number/{orderNumber}` - Get order by number
- `GET /api/orders/user/{userId}` - Get user orders
- `GET /api/orders/status/{status}` - Get orders by status
- `POST /api/orders` - Create order
- `PATCH /api/orders/{id}/status` - Update order status
- `GET /api/orders/sales/total?startDate=&endDate=` - Get total sales

## Database Tables (70+)

Key tables:
- Users, Roles, Permissions, RolePermissions
- Products, ProductImages, ProductVariants, ProductTags
- Categories (with hierarchical support)
- Orders, OrderItems
- Cart, Wishlists
- Reviews, Ratings
- Coupons, Campaigns
- Inventory, StockMovements
- Banners, Sliders, Blogs
- Couriers, Shipments, Tracking
- Notifications, AuditLogs
- Settings

## Configuration Files

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your Connection String"
  },
  "Jwt": {
    "SecretKey": "your-secret-key",
    "Issuer": "NamelyzeEcom",
    "Audience": "NamelyzeEcomUsers",
    "ExpirationMinutes": 60
  },
  "AppSettings": {
    "DefaultPageSize": 10,
    "MaxPageSize": 100,
    "LowStockThreshold": 10,
    "DefaultShippingChargeDhaka": 0,
    "DefaultShippingChargeOutsideDhaka": 100,
    "AllowedImageExtensions": ".jpg,.jpeg,.png,.gif",
    "MaxImageSizeInMB": 5
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-password",
    "EnableSSL": true
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

## Entity Relationships

### Product Management
- Product → Brand (Many-to-One)
- Product → Category (Many-to-One)
- Product → ProductImages (One-to-Many)
- Product → ProductVariants (One-to-Many)
- Product → ProductTags (One-to-Many)

### Order Management
- Order → User (Many-to-One)
- Order → OrderItems (One-to-Many)
- Order → Courier (Many-to-One)
- OrderItem → Product (Many-to-One)

### User Management
- User → Role (Many-to-One)
- User → Orders (One-to-Many)
- User → Reviews (One-to-Many)
- User → Wishlists (One-to-Many)

## Security Considerations

1. **Authentication**: JWT-based authentication
2. **Authorization**: Role-based access control (RBAC)
3. **Password Security**: Bcrypt hashing
4. **Data Protection**: HTTPS/SSL encryption
5. **SQL Injection Prevention**: Parameterized queries via EF Core
6. **XSS Protection**: Input validation
7. **CSRF Protection**: Token validation

## Development Workflow

```
Feature Branch
    ↓
Develop Branch (Testing)
    ↓
Main Branch (Production)
```

## Deployment Checklist

- [ ] Update connection strings for production
- [ ] Change JWT secret key
- [ ] Configure email settings
- [ ] Setup SMS provider credentials
- [ ] Enable HTTPS
- [ ] Configure firewall rules
- [ ] Setup logging
- [ ] Enable backup strategy
- [ ] Configure monitoring
- [ ] Test payment gateways

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [SQL Server Best Practices](https://docs.microsoft.com/sql/sql-server/tutorials)

## Support

For issues or questions, please create an issue on the GitHub repository.

---

**Last Updated**: June 2026
**Version**: 1.0
