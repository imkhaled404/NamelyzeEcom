namespace NamelyzeEcom.Data.Entities;

public class Inventory : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public int StockQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => StockQuantity - ReservedQuantity;
    public string? Location { get; set; }
}

public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public int Quantity { get; set; }
    public int Type { get; set; } // Purchase, Sale, Adjustment, Return
    public string? Reference { get; set; } // Order ID or Purchase ID
    public string? Remarks { get; set; }
}

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
