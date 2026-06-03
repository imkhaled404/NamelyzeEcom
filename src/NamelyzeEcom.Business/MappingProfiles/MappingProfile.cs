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
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand!.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name));
        CreateMap<CreateProductDto, Product>();

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
        CreateMap<CreateOrderDto, Order>();

        // OrderItem mappings
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name));
        CreateMap<CreateOrderItemDto, OrderItem>();

        // User mappings
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<RegisterUserDto, User>();

        // Category mappings
        CreateMap<Category, CategoryDto>().ReverseMap();

        // Coupon mappings
        CreateMap<Coupon, CouponDto>().ReverseMap();

        // Review mappings
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User!.FirstName + " " + src.User.LastName));
        CreateMap<CreateReviewDto, Review>();

        // ProductImage mappings
        CreateMap<ProductImage, ProductImageDto>().ReverseMap();

        // ProductVariant mappings
        CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();
    }
}
