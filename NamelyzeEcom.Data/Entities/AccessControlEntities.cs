namespace NamelyzeEcom.Data.Entities;

public class Menu : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int? ParentMenuId { get; set; }
    public virtual Menu? ParentMenu { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<MenuRolePermission> MenuRolePermissions { get; set; } = new HashSet<MenuRolePermission>();
    public virtual ICollection<Menu> SubMenus { get; set; } = new HashSet<Menu>();
}

public class MenuRolePermission : BaseEntity
{
    public int MenuId { get; set; }
    public virtual Menu? Menu { get; set; }
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
    public bool CanView { get; set; } = true;
}
