using Microsoft.AspNetCore.Mvc;
using NamelyzeEcom.Business.Interfaces;
using System.Security.Claims;

namespace NamelyzeEcom.Web.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;

    public SidebarViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.Identity!.IsAuthenticated)
            return View(new List<NamelyzeEcom.Business.DTOs.MenuDto>());

        var roleIdClaim = UserClaimsPrincipal.FindFirst("RoleId")?.Value;
        int roleId = int.Parse(roleIdClaim ?? "4"); // Default to Customer (4) if not found

        var result = await _menuService.GetMenusByRoleAsync(roleId);

        return View(result.Data ?? new List<NamelyzeEcom.Business.DTOs.MenuDto>());
    }
}
