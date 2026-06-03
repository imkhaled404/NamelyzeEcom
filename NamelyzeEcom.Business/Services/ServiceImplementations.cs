using Microsoft.EntityFrameworkCore;
using NamelyzeEcom.Business.DTOs;
using NamelyzeEcom.Business.Interfaces;
using NamelyzeEcom.Business.Helpers;
using NamelyzeEcom.Common.Utilities;
using NamelyzeEcom.Data.Entities;
using NamelyzeEcom.Data.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace NamelyzeEcom.Business.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResult<ProductDto>> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return ApiResult<ProductDto>.FailureResult("Product not found");
        return ApiResult<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product));
    }

    public async Task<ApiResult<ProductDto>> GetProductBySlugAsync(string slug)
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.Slug == slug);
        var product = products.FirstOrDefault();
        if (product == null) return ApiResult<ProductDto>.FailureResult("Product not found");
        return ApiResult<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product));
    }

    public async Task<ApiResult<IEnumerable<ProductDto>>> GetAllProductsAsync(int pageNumber, int pageSize)
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        var paged = products.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return ApiResult<IEnumerable<ProductDto>>.SuccessResult(_mapper.Map<IEnumerable<ProductDto>>(paged));
    }

    public async Task<ApiResult<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId);
        return ApiResult<IEnumerable<ProductDto>>.SuccessResult(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<ApiResult<IEnumerable<ProductDto>>> GetFeaturedProductsAsync()
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.IsFeatured);
        return ApiResult<IEnumerable<ProductDto>>.SuccessResult(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<ApiResult<IEnumerable<ProductDto>>> GetNewArrivalsAsync()
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.IsNew);
        return ApiResult<IEnumerable<ProductDto>>.SuccessResult(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<ApiResult<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.Name.Contains(searchTerm) || p.SKU.Contains(searchTerm));
        return ApiResult<IEnumerable<ProductDto>>.SuccessResult(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<ApiResult<ProductDto>> CreateProductAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        product.Slug = dto.Name.ToLower().Replace(" ", "-");
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.CompleteAsync();
        return ApiResult<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product));
    }

    public async Task<ApiResult<ProductDto>> UpdateProductAsync(int id, CreateProductDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return ApiResult<ProductDto>.FailureResult("Product not found");
        
        _mapper.Map(dto, product);
        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.CompleteAsync();
        return ApiResult<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product));
    }

    public async Task<ApiResult<bool>> DeleteProductAsync(int id)
    {
        await _unitOfWork.Products.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return ApiResult<bool>.SuccessResult(true);
    }

    public async Task<ApiResult<IEnumerable<ProductDto>>> GetRecommendedProductsAsync(int productId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null) return ApiResult<IEnumerable<ProductDto>>.FailureResult("Product not found");
        
        var recommendations = await _unitOfWork.Products.FindAsync(p => p.CategoryId == product.CategoryId && p.Id != productId);
        return ApiResult<IEnumerable<ProductDto>>.SuccessResult(_mapper.Map<IEnumerable<ProductDto>>(recommendations.Take(4)));
    }

    public async Task<ApiResult<IEnumerable<ProductDto>>> GetPersonalizedRecommendationsAsync(int userId, int limit = 8)
    {
        var activities = await _unitOfWork.UserActivities.FindAsync(a => a.UserId == userId);
        
        if (!activities.Any())
        {
            var featured = await GetFeaturedProductsAsync();
            return ApiResult<IEnumerable<ProductDto>>.SuccessResult(featured.Data!.Take(limit));
        }

        // Weighted algorithm: Purchase=10, Cart=5, View=1
        var categoryWeights = activities
            .GroupBy(a => a.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                Weight = g.Sum(a => 
                    (a.ActivityType == "Purchase" ? 10 : 
                     a.ActivityType == "AddToCart" ? 5 : 1) * a.Count)
            })
            .OrderByDescending(x => x.Weight)
            .Take(3)
            .Select(x => x.CategoryId)
            .ToList();

        var recommended = new List<Product>();
        foreach (var catId in categoryWeights)
        {
            if (catId.HasValue)
            {
                var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == catId.Value);
                recommended.AddRange(products.Take(limit / 2));
            }
        }

        var result = _mapper.Map<IEnumerable<ProductDto>>(recommended.DistinctBy(p => p.Id).Take(limit));
        return ApiResult<IEnumerable<ProductDto>>.SuccessResult(result);
    }

    public async Task<ApiResult<bool>> TrackActivityAsync(int userId, int productId, string activityType)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null) return ApiResult<bool>.FailureResult("Product not found");

        var existing = await _unitOfWork.UserActivities.FindAsync(a => 
            a.UserId == userId && a.ProductId == productId && a.ActivityType == activityType);
        
        var activity = existing.FirstOrDefault();
        if (activity != null)
        {
            activity.Count++;
            await _unitOfWork.UserActivities.UpdateAsync(activity);
        }
        else
        {
            await _unitOfWork.UserActivities.AddAsync(new UserActivity
            {
                UserId = userId,
                ProductId = productId,
                CategoryId = product.CategoryId,
                ActivityType = activityType,
                Count = 1
            });
        }

        await _unitOfWork.CompleteAsync();
        return ApiResult<bool>.SuccessResult(true);
    }

    public async Task<ApiResult<int>> BulkUploadAsync(byte[] fileData)
    {
        _logger.LogInformation("Starting bulk product upload");
        var csvContent = System.Text.Encoding.UTF8.GetString(fileData);
        var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int count = 0;

        foreach (var line in lines.Skip(1)) // Skip header
        {
            var parts = line.Split(',');
            if (parts.Length >= 3)
            {
                var product = new Product
                {
                    Name = parts[0],
                    SKU = parts[1],
                    RegularPrice = decimal.Parse(parts[2]),
                    SalePrice = parts.Length > 3 ? decimal.Parse(parts[3]) : decimal.Parse(parts[2]),
                    Slug = parts[0].ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString().Substring(0, 4),
                    IsActive = true
                };
                await _unitOfWork.Products.AddAsync(product);
                count++;
            }
        }

        await _unitOfWork.CompleteAsync();
        return ApiResult<int>.SuccessResult(count);
    }

    public async Task<ApiResult<int>> BulkPriceUpdateAsync(int? categoryId, string adjustmentType, decimal value)
    {
        _logger.LogInformation("Starting bulk price update for category {CategoryId}", categoryId);
        var products = categoryId.HasValue 
            ? await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId.Value)
            : await _unitOfWork.Products.GetAllAsync();

        int count = 0;
        foreach (var product in products)
        {
            if (adjustmentType.Contains("Percentage"))
            {
                var factor = value / 100;
                if (adjustmentType.Contains("Increase")) product.SalePrice *= (1 + factor);
                else product.SalePrice *= (1 - factor);
            }
            else
            {
                if (adjustmentType.Contains("Increase")) product.SalePrice += value;
                else product.SalePrice -= value;
            }
            
            await _unitOfWork.Products.UpdateAsync(product);
            count++;
        }

        await _unitOfWork.CompleteAsync();
        return ApiResult<int>.SuccessResult(count);
    }
}

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResult<OrderDto>> GetOrderByIdAsync(int id)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null) return ApiResult<OrderDto>.FailureResult("Order not found");
        return ApiResult<OrderDto>.SuccessResult(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResult<OrderDto>> GetOrderByNumberAsync(string orderNumber)
    {
        var orders = await _unitOfWork.Orders.FindAsync(o => o.OrderNumber == orderNumber);
        var order = orders.FirstOrDefault();
        if (order == null) return ApiResult<OrderDto>.FailureResult("Order not found");
        return ApiResult<OrderDto>.SuccessResult(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResult<IEnumerable<OrderDto>>> GetUserOrdersAsync(int userId)
    {
        var orders = await _unitOfWork.Orders.FindAsync(o => o.UserId == userId);
        return ApiResult<IEnumerable<OrderDto>>.SuccessResult(_mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    public async Task<ApiResult<IEnumerable<OrderDto>>> GetOrdersByStatusAsync(int status)
    {
        var orders = await _unitOfWork.Orders.FindAsync(o => o.OrderStatus == status);
        return ApiResult<IEnumerable<OrderDto>>.SuccessResult(_mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    public async Task<ApiResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto)
    {
        var orderNumber = $"ORD-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        var order = _mapper.Map<Order>(dto);
        order.OrderNumber = orderNumber;
        order.OrderStatus = 1; // Pending
        
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.CompleteAsync();
        return ApiResult<OrderDto>.SuccessResult(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResult<OrderDto>> UpdateOrderStatusAsync(int id, int status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null) return ApiResult<OrderDto>.FailureResult("Order not found");
        
        order.OrderStatus = status;
        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.CompleteAsync();
        return ApiResult<OrderDto>.SuccessResult(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResult<decimal>> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _unitOfWork.Orders.FindAsync(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.OrderStatus == 6); // Delivered
        return ApiResult<decimal>.SuccessResult(orders.Sum(o => o.TotalAmount));
    }

    public async Task<ApiResult<IEnumerable<OrderDto>>> GetAbandonedCartsAsync()
    {
        var yesterday = DateTime.UtcNow.AddDays(-1);
        // Logic: Orders in Pending (1) status created more than 24h ago
        var abandoned = await _unitOfWork.Orders.FindAsync(o => o.OrderStatus == 1 && o.CreatedAt <= yesterday);
        return ApiResult<IEnumerable<OrderDto>>.SuccessResult(_mapper.Map<IEnumerable<OrderDto>>(abandoned));
    }
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResult<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return ApiResult<UserDto>.FailureResult("User not found");
        return ApiResult<UserDto>.SuccessResult(_mapper.Map<UserDto>(user));
    }

    public async Task<ApiResult<UserDto>> GetUserByEmailAsync(string email)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == email);
        var user = users.FirstOrDefault();
        if (user == null) return ApiResult<UserDto>.FailureResult("User not found");
        return ApiResult<UserDto>.SuccessResult(_mapper.Map<UserDto>(user));
    }

    public async Task<ApiResult<UserDto>> RegisterAsync(RegisterUserDto dto)
    {
        var existing = await _unitOfWork.Users.FindAsync(u => u.Email == dto.Email.Trim());
        if (existing.Any()) return ApiResult<UserDto>.FailureResult("Email already registered");
        
        var user = _mapper.Map<User>(dto);
        user.PasswordHash = PasswordHasher.HashPassword(dto.Password.Trim());
        user.RoleId = 4; // Customer

        user.IsActive = true;
        
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        return ApiResult<UserDto>.SuccessResult(_mapper.Map<UserDto>(user));
    }

    public async Task<ApiResult<UserDto>> LoginAsync(LoginUserDto dto)
    {
        var email = dto.EmailOrPhone.Trim();
        var password = dto.Password.Trim();
        
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == email || u.PhoneNumber == email);
        var user = users.FirstOrDefault();
        
        if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
            return ApiResult<UserDto>.FailureResult("Invalid credentials");
            
        return ApiResult<UserDto>.SuccessResult(_mapper.Map<UserDto>(user));
    }

    public async Task<ApiResult<UserDto>> UpdateUserAsync(int id, UserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return ApiResult<UserDto>.FailureResult("User not found");
        
        _mapper.Map(dto, user);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
        return ApiResult<UserDto>.SuccessResult(_mapper.Map<UserDto>(user));
    }

    public async Task<ApiResult<bool>> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return ApiResult<bool>.FailureResult("User not found");
        
        if (!PasswordHasher.VerifyPassword(oldPassword.Trim(), user.PasswordHash))
            return ApiResult<bool>.FailureResult("Incorrect old password");
            
        user.PasswordHash = PasswordHasher.HashPassword(newPassword.Trim());
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
        return ApiResult<bool>.SuccessResult(true);
    }

    public async Task<ApiResult<IEnumerable<RoleDto>>> GetAllRolesAsync()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();
        return ApiResult<IEnumerable<RoleDto>>.SuccessResult(_mapper.Map<IEnumerable<RoleDto>>(roles));
    }

    public async Task<ApiResult<bool>> CreateRoleAsync(Role role)
    {
        await _unitOfWork.Roles.AddAsync(role);
        await _unitOfWork.CompleteAsync();
        return ApiResult<bool>.SuccessResult(true);
    }
}


public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResult<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return ApiResult<CategoryDto>.FailureResult("Category not found");
        return ApiResult<CategoryDto>.SuccessResult(_mapper.Map<CategoryDto>(category));
    }

    public async Task<ApiResult<CategoryDto>> GetCategoryBySlugAsync(string slug)
    {
        var categories = await _unitOfWork.Categories.FindAsync(c => c.Slug == slug);
        var category = categories.FirstOrDefault();
        if (category == null) return ApiResult<CategoryDto>.FailureResult("Category not found");
        return ApiResult<CategoryDto>.SuccessResult(_mapper.Map<CategoryDto>(category));
    }

    public async Task<ApiResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return ApiResult<IEnumerable<CategoryDto>>.SuccessResult(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<ApiResult<IEnumerable<CategoryDto>>> GetParentCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.FindAsync(c => c.ParentCategoryId == null);
        return ApiResult<IEnumerable<CategoryDto>>.SuccessResult(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<ApiResult<IEnumerable<CategoryDto>>> GetChildCategoriesAsync(int parentCategoryId)
    {
        var categories = await _unitOfWork.Categories.FindAsync(c => c.ParentCategoryId == parentCategoryId);
        return ApiResult<IEnumerable<CategoryDto>>.SuccessResult(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.CompleteAsync();
        return ApiResult<CategoryDto>.SuccessResult(_mapper.Map<CategoryDto>(category));
    }

    public async Task<ApiResult<CategoryDto>> UpdateCategoryAsync(int id, CategoryDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return ApiResult<CategoryDto>.FailureResult("Category not found");
        
        _mapper.Map(dto, category);
        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.CompleteAsync();
        return ApiResult<CategoryDto>.SuccessResult(_mapper.Map<CategoryDto>(category));
    }

    public async Task<ApiResult<bool>> DeleteCategoryAsync(int id)
    {
        await _unitOfWork.Categories.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return ApiResult<bool>.SuccessResult(true);
    }
}

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CouponService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResult<CouponDto>> GetCouponByIdAsync(int id)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
        if (coupon == null) return ApiResult<CouponDto>.FailureResult("Coupon not found");
        return ApiResult<CouponDto>.SuccessResult(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<ApiResult<CouponDto>> GetCouponByCodeAsync(string code)
    {
        var coupons = await _unitOfWork.Coupons.FindAsync(c => c.Code == code);
        var coupon = coupons.FirstOrDefault();
        if (coupon == null) return ApiResult<CouponDto>.FailureResult("Coupon not found");
        return ApiResult<CouponDto>.SuccessResult(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<ApiResult<IEnumerable<CouponDto>>> GetActiveCouponsAsync()
    {
        var coupons = await _unitOfWork.Coupons.FindAsync(c => c.IsActive && c.EndDate >= DateTime.UtcNow);
        return ApiResult<IEnumerable<CouponDto>>.SuccessResult(_mapper.Map<IEnumerable<CouponDto>>(coupons));
    }

    public async Task<ApiResult<CouponDto>> CreateCouponAsync(CouponDto dto)
    {
        var coupon = _mapper.Map<Coupon>(dto);
        await _unitOfWork.Coupons.AddAsync(coupon);
        await _unitOfWork.CompleteAsync();
        return ApiResult<CouponDto>.SuccessResult(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<ApiResult<CouponDto>> UpdateCouponAsync(int id, CouponDto dto)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
        if (coupon == null) return ApiResult<CouponDto>.FailureResult("Coupon not found");
        
        _mapper.Map(dto, coupon);
        await _unitOfWork.Coupons.UpdateAsync(coupon);
        await _unitOfWork.CompleteAsync();
        return ApiResult<CouponDto>.SuccessResult(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<ApiResult<bool>> ValidateCouponAsync(string code, decimal orderAmount)
    {
        var coupons = await _unitOfWork.Coupons.FindAsync(c => c.Code == code && c.IsActive && c.EndDate >= DateTime.UtcNow);
        var coupon = coupons.FirstOrDefault();
        
        if (coupon == null) return ApiResult<bool>.FailureResult("Invalid or expired coupon");
        if (orderAmount < coupon.MinimumOrderAmount) return ApiResult<bool>.FailureResult($"Minimum order amount for this coupon is {coupon.MinimumOrderAmount}");
        
        return ApiResult<bool>.SuccessResult(true);
    }
}

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResult<ReviewDto>> GetReviewByIdAsync(int id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null) return ApiResult<ReviewDto>.FailureResult("Review not found");
        return ApiResult<ReviewDto>.SuccessResult(_mapper.Map<ReviewDto>(review));
    }

    public async Task<ApiResult<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId)
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => r.ProductId == productId && r.IsApproved);
        return ApiResult<IEnumerable<ReviewDto>>.SuccessResult(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
    }

    public async Task<ApiResult<IEnumerable<ReviewDto>>> GetPendingReviewsAsync()
    {
        var reviews = await _unitOfWork.Reviews.FindAsync(r => !r.IsApproved);
        return ApiResult<IEnumerable<ReviewDto>>.SuccessResult(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
    }

    public async Task<ApiResult<ReviewDto>> CreateReviewAsync(CreateReviewDto dto, int userId)
    {
        var review = _mapper.Map<Review>(dto);
        review.UserId = userId;
        review.IsApproved = false;
        
        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.CompleteAsync();
        return ApiResult<ReviewDto>.SuccessResult(_mapper.Map<ReviewDto>(review));
    }

    public async Task<ApiResult<ReviewDto>> ApproveReviewAsync(int id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null) return ApiResult<ReviewDto>.FailureResult("Review not found");
        
        review.IsApproved = true;
        await _unitOfWork.Reviews.UpdateAsync(review);
        await _unitOfWork.CompleteAsync();
        return ApiResult<ReviewDto>.SuccessResult(_mapper.Map<ReviewDto>(review));
    }

    public async Task<ApiResult<ReviewDto>> RejectReviewAsync(int id)
    {
        await _unitOfWork.Reviews.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return ApiResult<ReviewDto>.SuccessResult(null!);
    }

    public async Task<ApiResult<bool>> DeleteReviewAsync(int id)
    {
        await _unitOfWork.Reviews.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return ApiResult<bool>.SuccessResult(true);
    }
}

public class CourierService : ICourierService
{
    public async Task<ApiResult<string>> CreateConsignmentAsync(OrderDto order, string provider)
    {
        return ApiResult<string>.SuccessResult($"CONS-{Guid.NewGuid().ToString().ToUpper().Substring(0, 8)}");
    }

    public async Task<ApiResult<string>> GetTrackingStatusAsync(string consignmentId, string provider)
    {
        return ApiResult<string>.SuccessResult("In Transit");
    }
}

public class NotificationService : INotificationService
{
    public async Task<ApiResult<bool>> SendSmsAsync(string phone, string message)
    {
        return ApiResult<bool>.SuccessResult(true);
    }

    public async Task<ApiResult<bool>> SendEmailAsync(string email, string subject, string body)
    {
        return ApiResult<bool>.SuccessResult(true);
    }
}
