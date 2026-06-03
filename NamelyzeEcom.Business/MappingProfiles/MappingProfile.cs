using AutoMapper;
using NamelyzeEcom.Business.DTOs;
using NamelyzeEcom.Data.Entities;

namespace NamelyzeEcom.Business.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => 
                src.ProductImages.Any(i => i.IsMain) ? src.ProductImages.First(i => i.IsMain).ImageUrl : 
                (src.ProductImages.Any() ? src.ProductImages.First().ImageUrl : null)));
        CreateMap<CreateProductDto, Product>();
        CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null));
        CreateMap<UserDto, User>();
        CreateMap<RegisterUserDto, User>();

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
        CreateMap<CreateOrderDto, Order>();
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

        // Category mappings
        CreateMap<Category, CategoryDto>().ReverseMap();

        // Coupon mappings
        CreateMap<Coupon, CouponDto>().ReverseMap();

        // Review mappings
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : "Unknown"));
        CreateMap<CreateReviewDto, Review>();

        // Menu mappings
        CreateMap<Menu, MenuDto>().ReverseMap();
        
        // Address mappings
        CreateMap<UserAddress, UserAddressDto>().ReverseMap();

        // Loyalty Point mappings
        CreateMap<LoyaltyPoint, LoyaltyPointDto>().ReverseMap();

        // Role mapping
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}
