# NamelyzeEcom - Dynamic E-Commerce Platform

NamelyzeEcom is a full-featured, production-ready e-commerce platform built with ASP.NET Core 9. It follows a clean, layered architecture and includes modules for product management, identity, orders, inventory, and marketing.

## 🚀 Key Features

*   **Multilayered Architecture**: Clean separation of concerns (Web, Business, Data, Common).
*   **Comprehensive Domain Model**: Over 70 tables covering all aspects of e-commerce.
*   **Secure Authentication**: JWT-based authentication with Role-Based Access Control (RBAC).
*   **Advanced Product Management**: Hierarchical categories, variants, and SEO optimization.
*   **Order Workflow**: 9-stage order status management.
*   **Inventory & Stock Tracking**: Real-time stock management with logs.
*   **Modern API**: RESTful endpoints documented with Swagger.

## 🛠️ Technology Stack

*   **Backend**: ASP.NET Core 9 Web API
*   **Database**: SQL Server 2019+
*   **ORM**: Entity Framework Core
*   **Authentication**: JWT & BCrypt for hashing
*   **Mapping**: AutoMapper
*   **Documentation**: Swagger (OpenAPI)

## 📁 Project Structure

*   `src/NamelyzeEcom.Web`: API Layer (Controllers, Config)
*   `src/NamelyzeEcom.Business`: Logic Layer (Services, DTOs, Mappings)
*   `src/NamelyzeEcom.Data`: Data Layer (Entities, Context, Repositories)
*   `src/NamelyzeEcom.Common`: Shared Layer (Enums, Constants, Utilities)
*   `Database/Schema`: SQL scripts for database initialization.

## 🏁 Getting Started

1.  **Clone the repository**.
2.  **Initialize Database**:
    *   Run the script located at `Database/Schema/01-Initial-Schema.sql` in your SQL Server instance.
3.  **Configure Application**:
    *   Update the connection string in `src/NamelyzeEcom.Web/appsettings.json`.
4.  **Run the Application**:
    ```bash
    dotnet run --project src/NamelyzeEcom.Web
    ```
5.  **Explore API**:
    *   Go to `https://localhost:5001/swagger` to view the API documentation.

## 📄 Documentation

Detailed documentation can be found in the `/Documentation` folder:
*   [Setup Guide](Documentation/SETUP_GUIDE.md)
*   [API Reference](Documentation/API_DOCUMENTATION.md)
*   [Development Standards](Documentation/DEVELOPMENT_GUIDE.md)

## 📄 License

Proprietary and Confidential.