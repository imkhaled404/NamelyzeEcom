using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NamelyzeEcom.Business.Interfaces;
using NamelyzeEcom.Business.MappingProfiles;
using NamelyzeEcom.Business.Services;
using NamelyzeEcom.Data.Context;
using NamelyzeEcom.Data.UnitOfWork;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using NamelyzeEcom.Business.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NamelyzeEcom API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Database
builder.Services.AddDbContext<NamelyzeEcomContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("SecretKey");
var key = Encoding.ASCII.GetBytes(secretKey ?? "fallback_secret_key_for_dev_only");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICourierService>(sp => sp.GetRequiredService<CourierService>());
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try 
    {
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        // 0. Seed Roles first
        var roles = await unitOfWork.Roles.GetAllAsync();
        if (!roles.Any())
        {
            logger.LogInformation("Seeding roles...");
            var initialRoles = new List<NamelyzeEcom.Data.Entities.Role>
            {
                new() { Name = "SuperAdmin", Description = "System Owner" },
                new() { Name = "Admin", Description = "Administrator" },
                new() { Name = "Manager", Description = "Shop Manager" },
                new() { Name = "Customer", Description = "Registered Customer" },
                new() { Name = "CustomerSupport", Description = "Handle customer inquiries" },
                new() { Name = "InventoryManager", Description = "Manage products and stock" },
                new() { Name = "DeliveryMan", Description = "Handle pickups and deliveries" }
            };
            foreach(var role in initialRoles) await unitOfWork.Roles.AddAsync(role);
            await unitOfWork.CompleteAsync();
            logger.LogInformation("Roles seeded successfully.");
        }

        var superAdminEmail = "superadmin@namelyze.com";
        var adminEmail = "admin@namelyze.com";
        var adminPass = "Admin@123";
        
        // Seed SuperAdmin
        var superAdminResult = await userService.GetUserByEmailAsync(superAdminEmail);
        if (!superAdminResult.Success)
        {
            logger.LogInformation("Creating default SuperAdmin user...");
            await userService.RegisterAsync(new RegisterUserDto
            {
                FirstName = "System",
                LastName = "Owner",
                Email = superAdminEmail,
                PhoneNumber = "01811111111",
                Password = adminPass,
                ConfirmPassword = adminPass
            });
            var user = (await unitOfWork.Users.FindAsync(u => u.Email == superAdminEmail)).First();
            user.RoleId = 1; // SuperAdmin
            user.PasswordHash = NamelyzeEcom.Business.Helpers.PasswordHasher.HashPassword(adminPass);
            await unitOfWork.Users.UpdateAsync(user);
            await unitOfWork.CompleteAsync();
        }

        var usersResult = await userService.GetUserByEmailAsync(adminEmail);
        if (!usersResult.Success) 
        {
            logger.LogInformation("Creating default Admin user...");
            await userService.RegisterAsync(new RegisterUserDto
            {
                FirstName = "System",
                LastName = "Admin",
                Email = adminEmail,
                PhoneNumber = "01800000000",
                Password = adminPass,
                ConfirmPassword = adminPass
            });
        }

        // Always ensure the password and role are correct for the admin user
        var adminUser = await unitOfWork.Users.FindAsync(u => u.Email == adminEmail);
        var userEntity = adminUser.FirstOrDefault();
        if (userEntity != null)
        {
            userEntity.PasswordHash = NamelyzeEcom.Business.Helpers.PasswordHasher.HashPassword(adminPass);
            userEntity.RoleId = 2; // Admin
            userEntity.IsActive = true;
            await unitOfWork.Users.UpdateAsync(userEntity);
            await unitOfWork.CompleteAsync();
            logger.LogInformation("Admin account synchronized with SHA256 successfully.");
        }
        // 2. Seed Categories
        var categories = await unitOfWork.Categories.GetAllAsync();
        if (!categories.Any())
        {
            logger.LogInformation("Seeding initial categories...");
            var initialCats = new List<NamelyzeEcom.Data.Entities.Category>
            {
                new() { Name = "Home Appliances", Slug = "home-appliances", Image = "https://placehold.co/100" },
                new() { Name = "Gadgets & Electronics", Slug = "gadgets", Image = "https://placehold.co/100" },
                new() { Name = "Kitchen Appliances", Slug = "kitchen", Image = "https://placehold.co/100" },
                new() { Name = "Car Accessories", Slug = "car-acc", Image = "https://placehold.co/100" },
                new() { Name = "Clothing & Fashion", Slug = "fashion", Image = "https://placehold.co/100" }
            };
            foreach(var cat in initialCats) await unitOfWork.Categories.AddAsync(cat);
            await unitOfWork.CompleteAsync();
            categories = await unitOfWork.Categories.GetAllAsync();
        }

        // 3. Seed Professional Products
        var products = await unitOfWork.Products.GetAllAsync();
        if (!products.Any())
        {
            logger.LogInformation("Seeding premium products...");
            var catList = categories.ToList();
            var homeId = catList.First(c => c.Slug == "home-appliances").Id;
            var gadgetId = catList.First(c => c.Slug == "gadgets").Id;
            var kitchenId = catList.First(c => c.Slug == "kitchen").Id;
            var carId = catList.First(c => c.Slug == "car-acc").Id;
            var fashionId = catList.First(c => c.Slug == "fashion").Id;
            var demoProducts = new List<NamelyzeEcom.Data.Entities.Product>
            {
                // Fashion
                new() { Name = "Cotton Panjabi For Men - White", Slug = "white-cotton-panjabi", Description = "Premium white cotton Panjabi for elegant traditional wear.", SKU = "CLOTH-001", PurchasePrice = 800, RegularPrice = 1800, SalePrice = 1500, CurrentStock = 50, IsNew = true, IsFeatured = true, IsActive = true, CategoryId = fashionId },
                new() { Name = "Men's Slim Fit Shirt - Blue", Slug = "slim-fit-shirt-blue", Description = "Stylish slim fit shirt for formal and casual occasions.", SKU = "CLOTH-002", PurchasePrice = 600, RegularPrice = 1400, SalePrice = 1100, CurrentStock = 40, IsNew = true, IsActive = true, CategoryId = fashionId },
                new() { Name = "Ladies Floral Kurti - Pink", Slug = "ladies-floral-kurti-pink", Description = "Beautiful floral print kurti for modern women.", SKU = "CLOTH-003", PurchasePrice = 700, RegularPrice = 1600, SalePrice = 1300, CurrentStock = 35, IsFeatured = true, IsActive = true, CategoryId = fashionId },
                new() { Name = "Kid Fatua Set - Yellow", Slug = "kid-fatua-yellow", Description = "Vibrant yellow fatua for kids on special occasions.", SKU = "CLOTH-004", PurchasePrice = 400, RegularPrice = 900, SalePrice = 750, CurrentStock = 60, IsActive = true, CategoryId = fashionId },
                // Kitchen
                new() { Name = "Portable Electric Grinder", Slug = "electric-grinder", Description = "High-speed portable electric grinder for kitchen use.", SKU = "KITCHEN-001", PurchasePrice = 1200, RegularPrice = 2800, SalePrice = 2200, CurrentStock = 30, IsTrending = true, IsActive = true, CategoryId = kitchenId },
                new() { Name = "Non-stick Frying Pan Set 3PC", Slug = "non-stick-pan-set", Description = "Durable 3-piece non-stick frying pan set for healthy cooking.", SKU = "KITCHEN-002", PurchasePrice = 900, RegularPrice = 2200, SalePrice = 1800, CurrentStock = 25, IsFeatured = true, IsActive = true, CategoryId = kitchenId },
                new() { Name = "Electric Kettle 1.8L", Slug = "electric-kettle-1-8l", Description = "Fast boil 1.8L stainless steel electric kettle.", SKU = "KITCHEN-003", PurchasePrice = 800, RegularPrice = 1900, SalePrice = 1500, CurrentStock = 40, IsNew = true, IsActive = true, CategoryId = kitchenId },
                new() { Name = "Manual Blender Hand Mixer", Slug = "hand-blender", Description = "Powerful hand blender for smoothies and soups.", SKU = "KITCHEN-004", PurchasePrice = 1500, RegularPrice = 3500, SalePrice = 2800, CurrentStock = 20, IsTrending = true, IsActive = true, CategoryId = kitchenId },
                // Gadgets
                new() { Name = "Smart Watch Series 9", Slug = "smart-watch-s9", Description = "Advanced smartwatch with health tracking and amoled display.", SKU = "GADGET-001", PurchasePrice = 2000, RegularPrice = 4500, SalePrice = 3500, CurrentStock = 25, IsBestSeller = true, IsActive = true, CategoryId = gadgetId },
                new() { Name = "True Wireless Earbuds TWS", Slug = "tws-earbuds", Description = "Noise cancelling true wireless earbuds with 30hr battery.", SKU = "GADGET-002", PurchasePrice = 1500, RegularPrice = 3500, SalePrice = 2800, CurrentStock = 50, IsFeatured = true, IsActive = true, CategoryId = gadgetId },
                new() { Name = "Portable Power Bank 20000mAh", Slug = "powerbank-20000", Description = "20000mAh fast charging power bank with dual USB-C.", SKU = "GADGET-003", PurchasePrice = 800, RegularPrice = 2000, SalePrice = 1600, CurrentStock = 45, IsNew = true, IsBestSeller = true, IsActive = true, CategoryId = gadgetId },
                new() { Name = "Ring Light 12 inch Selfie", Slug = "ring-light-12", Description = "Professional 12 inch ring light for video calls and photography.", SKU = "GADGET-004", PurchasePrice = 1200, RegularPrice = 2800, SalePrice = 2200, CurrentStock = 30, IsTrending = true, IsActive = true, CategoryId = gadgetId },
                // Home Appliances
                new() { Name = "Wireless Air Fryer 5L", Slug = "air-fryer-wireless", Description = "Cook healthy meals with our large capacity wireless air fryer.", SKU = "HOME-001", PurchasePrice = 4500, RegularPrice = 9500, SalePrice = 7500, CurrentStock = 15, IsFeatured = true, IsActive = true, CategoryId = homeId },
                new() { Name = "Robot Vacuum Cleaner Auto", Slug = "robot-vacuum", Description = "Smart robot vacuum with auto-mapping and app control.", SKU = "HOME-002", PurchasePrice = 8000, RegularPrice = 18000, SalePrice = 14000, CurrentStock = 10, IsBestSeller = true, IsActive = true, CategoryId = homeId },
                new() { Name = "Portable Rechargeable Fan", Slug = "rechargeable-fan", Description = "USB rechargeable table fan with 3 speed modes.", SKU = "HOME-003", PurchasePrice = 600, RegularPrice = 1400, SalePrice = 1100, CurrentStock = 60, IsNew = true, IsActive = true, CategoryId = homeId },
                new() { Name = "Water Purifier 7-Stage Filter", Slug = "water-purifier-7stage", Description = "Advanced 7-stage water purification system.", SKU = "HOME-004", PurchasePrice = 5000, RegularPrice = 12000, SalePrice = 9500, CurrentStock = 12, IsFeatured = true, IsActive = true, CategoryId = homeId },
                // Car Accessories
                new() { Name = "Car Wireless Charger Mount", Slug = "car-wireless-charger", Description = "10W wireless car charger with one-touch mounting.", SKU = "CAR-001", PurchasePrice = 700, RegularPrice = 1800, SalePrice = 1400, CurrentStock = 40, IsNew = true, IsActive = true, CategoryId = carId },
                new() { Name = "Dash Cam Full HD 1080P", Slug = "dash-cam-1080p", Description = "Wide angle full HD dashboard camera with night vision.", SKU = "CAR-002", PurchasePrice = 2000, RegularPrice = 4800, SalePrice = 3800, CurrentStock = 20, IsBestSeller = true, IsActive = true, CategoryId = carId },
                new() { Name = "Car Neck Pillow Support", Slug = "car-neck-pillow", Description = "Memory foam lumbar and neck support pillow for car seats.", SKU = "CAR-003", PurchasePrice = 400, RegularPrice = 900, SalePrice = 750, CurrentStock = 55, IsActive = true, CategoryId = carId },
                new() { Name = "Universal Car Phone Holder", Slug = "car-phone-holder", Description = "360° rotating windshield car phone mount for all phones.", SKU = "CAR-004", PurchasePrice = 200, RegularPrice = 600, SalePrice = 480, CurrentStock = 80, IsNew = true, IsActive = true, CategoryId = carId }
            };

            foreach(var prod in demoProducts) await unitOfWork.Products.AddAsync(prod);
            await unitOfWork.CompleteAsync();
            logger.LogInformation("Seeding complete with professional data.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database synchronization.");
    }
}


app.Run();




