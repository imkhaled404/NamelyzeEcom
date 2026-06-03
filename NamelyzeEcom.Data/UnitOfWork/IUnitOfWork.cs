using NamelyzeEcom.Data.Entities;
using NamelyzeEcom.Data.Repositories.Interfaces;

namespace NamelyzeEcom.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<ProductImage> ProductImages { get; }
    IRepository<Order> Orders { get; }
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Category> Categories { get; }

    IRepository<Brand> Brands { get; }
    IRepository<Coupon> Coupons { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Inventory> Inventories { get; }
    IRepository<StockMovement> StockMovements { get; }
    IRepository<Blog> Blogs { get; }
    IRepository<Banner> Banners { get; }
    IRepository<Slider> Sliders { get; }
    IRepository<Setting> Settings { get; }
    IRepository<Campaign> Campaigns { get; }
    IRepository<Menu> Menus { get; }
    IRepository<MenuRolePermission> MenuRolePermissions { get; }
    IRepository<UserAddress> UserAddresses { get; }
    IRepository<LoyaltyPoint> LoyaltyPoints { get; }
    IRepository<UserActivity> UserActivities { get; }

    Task<int> CompleteAsync();
}
