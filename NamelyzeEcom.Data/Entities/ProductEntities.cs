namespace NamelyzeEcom.Data.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal RegularPrice { get; set; }
    public decimal SalePrice { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public bool IsNew { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsTrending { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsActive { get; set; } = true;
    public int Status { get; set; }

    
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
    
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new HashSet<ProductVariant>();
    public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    
    public decimal AverageRating => Reviews.Any() ? (decimal)Reviews.Average(r => r.Rating) : 0;
    public int TotalReviews => Reviews.Count;
}

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    public bool IsActive { get; set; } = true;
}

public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
}

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public string Color { get; set; } = string.Empty;
    public string? Size { get; set; }
    public string? Weight { get; set; }
    public string? Capacity { get; set; }
    public decimal AdditionalPrice { get; set; }
    public int StockCount { get; set; }
}

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }
}
