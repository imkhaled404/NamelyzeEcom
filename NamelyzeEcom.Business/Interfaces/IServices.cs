using NamelyzeEcom.Business.DTOs;
using NamelyzeEcom.Common.Utilities;
using NamelyzeEcom.Data.Entities;

namespace NamelyzeEcom.Business.Interfaces;

public interface IProductService
{
    Task<ApiResult<ProductDto>> GetProductByIdAsync(int id);
    Task<ApiResult<ProductDto>> GetProductBySlugAsync(string slug);
    Task<ApiResult<IEnumerable<ProductDto>>> GetAllProductsAsync(int pageNumber, int pageSize);
    Task<ApiResult<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(int categoryId);
    Task<ApiResult<IEnumerable<ProductDto>>> GetFeaturedProductsAsync();
    Task<ApiResult<IEnumerable<ProductDto>>> GetNewArrivalsAsync();
    Task<ApiResult<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm);
    Task<ApiResult<ProductDto>> CreateProductAsync(CreateProductDto dto);
    Task<ApiResult<ProductDto>> UpdateProductAsync(int id, CreateProductDto dto);
    Task<ApiResult<bool>> DeleteProductAsync(int id);
    Task<ApiResult<IEnumerable<ProductDto>>> GetRecommendedProductsAsync(int productId); // AI Recommendation placeholder
    Task<ApiResult<IEnumerable<ProductDto>>> GetPersonalizedRecommendationsAsync(int userId, int limit = 8);
    Task<ApiResult<bool>> TrackActivityAsync(int userId, int productId, string activityType);
    Task<ApiResult<int>> BulkUploadAsync(byte[] fileData); // Returns count of imported products
    Task<ApiResult<int>> BulkPriceUpdateAsync(int? categoryId, string adjustmentType, decimal value); // Returns count of updated products
}

public interface IOrderService
{
    Task<ApiResult<OrderDto>> GetOrderByIdAsync(int id);
    Task<ApiResult<OrderDto>> GetOrderByNumberAsync(string orderNumber);
    Task<ApiResult<IEnumerable<OrderDto>>> GetUserOrdersAsync(int userId);
    Task<ApiResult<IEnumerable<OrderDto>>> GetOrdersByStatusAsync(int status);
    Task<ApiResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto);
    Task<ApiResult<OrderDto>> UpdateOrderStatusAsync(int id, int status);
    Task<ApiResult<decimal>> GetTotalSalesAsync(DateTime startDate, DateTime endDate);
    Task<ApiResult<IEnumerable<OrderDto>>> GetAbandonedCartsAsync(); // Abandoned Cart Recovery logic
}

public interface IUserService
{
    Task<ApiResult<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResult<UserDto>> GetUserByEmailAsync(string email);
    Task<ApiResult<UserDto>> RegisterAsync(RegisterUserDto dto);
    Task<ApiResult<UserDto>> LoginAsync(LoginUserDto dto);
    Task<ApiResult<UserDto>> UpdateUserAsync(int id, UserDto dto);
    Task<ApiResult<bool>> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<ApiResult<IEnumerable<RoleDto>>> GetAllRolesAsync();
    Task<ApiResult<bool>> CreateRoleAsync(Role role);
}


public interface ICategoryService
{
    Task<ApiResult<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<ApiResult<CategoryDto>> GetCategoryBySlugAsync(string slug);
    Task<ApiResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
    Task<ApiResult<IEnumerable<CategoryDto>>> GetParentCategoriesAsync();
    Task<ApiResult<IEnumerable<CategoryDto>>> GetChildCategoriesAsync(int parentCategoryId);
    Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryDto dto);
    Task<ApiResult<CategoryDto>> UpdateCategoryAsync(int id, CategoryDto dto);
    Task<ApiResult<bool>> DeleteCategoryAsync(int id);
}

public interface ICouponService
{
    Task<ApiResult<CouponDto>> GetCouponByIdAsync(int id);
    Task<ApiResult<CouponDto>> GetCouponByCodeAsync(string code);
    Task<ApiResult<IEnumerable<CouponDto>>> GetActiveCouponsAsync();
    Task<ApiResult<CouponDto>> CreateCouponAsync(CouponDto dto);
    Task<ApiResult<CouponDto>> UpdateCouponAsync(int id, CouponDto dto);
    Task<ApiResult<bool>> ValidateCouponAsync(string code, decimal orderAmount);
}

public interface IReviewService
{
    Task<ApiResult<ReviewDto>> GetReviewByIdAsync(int id);
    Task<ApiResult<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId);
    Task<ApiResult<IEnumerable<ReviewDto>>> GetPendingReviewsAsync();
    Task<ApiResult<ReviewDto>> CreateReviewAsync(CreateReviewDto dto, int userId);
    Task<ApiResult<ReviewDto>> ApproveReviewAsync(int id);
    Task<ApiResult<ReviewDto>> RejectReviewAsync(int id);
    Task<ApiResult<bool>> DeleteReviewAsync(int id);
}

public interface ICourierService
{
    Task<ApiResult<string>> CreateConsignmentAsync(OrderDto order, string provider);
    Task<ApiResult<string>> GetTrackingStatusAsync(string consignmentId, string provider);
}

public interface INotificationService
{
    Task<ApiResult<bool>> SendSmsAsync(string phone, string message);
    Task<ApiResult<bool>> SendEmailAsync(string email, string subject, string body);
}
