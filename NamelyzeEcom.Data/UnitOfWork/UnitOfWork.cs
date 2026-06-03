using NamelyzeEcom.Data.Context;
using NamelyzeEcom.Data.Entities;
using NamelyzeEcom.Data.Repositories;
using NamelyzeEcom.Data.Repositories.Interfaces;

namespace NamelyzeEcom.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly NamelyzeEcomContext _context;

    public UnitOfWork(NamelyzeEcomContext context)
    {
        _context = context;
        Products = new Repository<Product>(_context);
        ProductImages = new Repository<ProductImage>(_context);
        Orders = new Repository<Order>(_context);
        Users = new Repository<User>(_context);
        Roles = new Repository<Role>(_context);
        Categories = new Repository<Category>(_context);

        Brands = new Repository<Brand>(_context);
        Coupons = new Repository<Coupon>(_context);
        Reviews = new Repository<Review>(_context);
        Inventories = new Repository<Inventory>(_context);
        StockMovements = new Repository<StockMovement>(_context);
        Blogs = new Repository<Blog>(_context);
        Banners = new Repository<Banner>(_context);
        Sliders = new Repository<Slider>(_context);
        Settings = new Repository<Setting>(_context);
        Campaigns = new Repository<Campaign>(_context);
        Menus = new Repository<Menu>(_context);
        MenuRolePermissions = new Repository<MenuRolePermission>(_context);
        UserAddresses = new Repository<UserAddress>(_context);
        LoyaltyPoints = new Repository<LoyaltyPoint>(_context);
        UserActivities = new Repository<UserActivity>(_context);
    }

    public IRepository<Product> Products { get; private set; }
    public IRepository<ProductImage> ProductImages { get; private set; }
    public IRepository<Order> Orders { get; private set; }
    public IRepository<User> Users { get; private set; }
    public IRepository<Role> Roles { get; private set; }
    public IRepository<Category> Categories { get; private set; }

    public IRepository<Brand> Brands { get; private set; }
    public IRepository<Coupon> Coupons { get; private set; }
    public IRepository<Review> Reviews { get; private set; }
    public IRepository<Inventory> Inventories { get; private set; }
    public IRepository<StockMovement> StockMovements { get; private set; }
    public IRepository<Blog> Blogs { get; private set; }
    public IRepository<Banner> Banners { get; private set; }
    public IRepository<Slider> Sliders { get; private set; }
    public IRepository<Setting> Settings { get; private set; }
    public IRepository<Campaign> Campaigns { get; private set; }
    public IRepository<Menu> Menus { get; private set; }
    public IRepository<MenuRolePermission> MenuRolePermissions { get; private set; }
    public IRepository<UserAddress> UserAddresses { get; private set; }
    public IRepository<LoyaltyPoint> LoyaltyPoints { get; private set; }
    public IRepository<UserActivity> UserActivities { get; private set; }


    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
