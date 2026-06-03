namespace NamelyzeEcom.Data.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    
    public string ShippingType { get; set; } = "Regular";
    public decimal ShippingCharge { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PayableAmount { get; set; }
    
    public int OrderStatus { get; set; }
    public int PaymentStatus { get; set; }
    public int PaymentMethodId { get; set; }
    public string? CouponCode { get; set; }
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? VariantColor { get; set; }
    public string? VariantSize { get; set; }
}

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethodId { get; set; }
    public string? TransactionId { get; set; }
    public int Status { get; set; }
}

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public int Type { get; set; } // Percentage or Fixed
    public decimal Value { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }
}

public class Wishlist : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
}

public class Cart : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public int Quantity { get; set; }
    public string? VariantColor { get; set; }
    public string? VariantSize { get; set; }
}
