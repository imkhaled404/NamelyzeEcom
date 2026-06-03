namespace NamelyzeEcom.Data.Entities;

public class UserAddress : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public string Label { get; set; } = string.Empty; // Home, Office, etc.
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
}

public class LoyaltyPoint : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int Points { get; set; } = 0;
}
