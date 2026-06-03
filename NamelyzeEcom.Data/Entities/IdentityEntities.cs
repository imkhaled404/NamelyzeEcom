namespace NamelyzeEcom.Data.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();
    public virtual ICollection<Cart> Carts { get; set; } = new HashSet<Cart>();
}

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}

public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
    public int PermissionId { get; set; }
    public virtual Permission? Permission { get; set; }
}

public class AuditLog : BaseEntity
{
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
}

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}

public class UserActivity : BaseEntity
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int? ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public int? CategoryId { get; set; }
    public string ActivityType { get; set; } = string.Empty; // View, AddToCart, Purchase
    public int Count { get; set; } = 1;
}
