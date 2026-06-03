namespace NamelyzeEcom.Common.Enums;

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Packed = 4,
    Shipped = 5,
    Delivered = 6,
    Returned = 7,
    Cancelled = 8,
    Refunded = 9
}

public enum PaymentStatus
{
    Unpaid = 1,
    Paid = 2,
    PartiallyPaid = 3,
    Failed = 4,
    Refunded = 5
}

public enum PaymentMethod
{
    CashOnDelivery = 1,
    bKash = 2,
    Nagad = 3,
    Rocket = 4,
    SSLCommerz = 5,
    Visa = 6,
    MasterCard = 7
}

public enum CourierProvider
{
    SteadFast = 1,
    Pathao = 2,
    RedX = 3,
    eCourier = 4
}

public enum UserRole
{
    SuperAdmin = 1,
    Admin = 2,
    Manager = 3,
    InventoryOfficer = 4,
    CustomerSupport = 5,
    MarketingOfficer = 6,
    Accountant = 7,
    Customer = 8
}

public enum ProductStatus
{
    Active = 1,
    Inactive = 2,
    OutOfStock = 3
}

public enum CouponType
{
    Percentage = 1,
    FixedAmount = 2
}

public enum StockMovementType
{
    Purchase = 1,
    Sale = 2,
    Adjustment = 3,
    Return = 4
}
