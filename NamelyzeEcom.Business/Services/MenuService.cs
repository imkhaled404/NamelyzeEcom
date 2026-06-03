using NamelyzeEcom.Business.DTOs;
using NamelyzeEcom.Business.Interfaces;
using NamelyzeEcom.Common.Utilities;
using NamelyzeEcom.Data.Entities;
using NamelyzeEcom.Data.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace NamelyzeEcom.Business.Services;

public class MenuService : IMenuService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MenuService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResult<IEnumerable<MenuDto>>> GetMenusByRoleAsync(int roleId)
    {
        // Fetch all menu permissions for this role
        var permissions = await _unitOfWork.MenuRolePermissions.FindAsync(p => p.RoleId == roleId && p.CanView);
        var menuIds = permissions.Select(p => p.MenuId).ToList();

        // Fetch the actual menus
        var menus = await _unitOfWork.Menus.FindAsync(m => menuIds.Contains(m.Id) && m.IsActive);
        
        // Organize into hierarchy (simplified for now)
        var topLevelMenus = menus.Where(m => m.ParentMenuId == null).OrderBy(m => m.SortOrder).ToList();
        var dtos = _mapper.Map<IEnumerable<MenuDto>>(topLevelMenus);

        return ApiResult<IEnumerable<MenuDto>>.SuccessResult(dtos);
    }

    public async Task<ApiResult<MenuDto>> CreateMenuAsync(MenuDto dto)
    {
        var menu = _mapper.Map<Menu>(dto);
        await _unitOfWork.Menus.AddAsync(menu);
        await _unitOfWork.CompleteAsync();
        return ApiResult<MenuDto>.SuccessResult(_mapper.Map<MenuDto>(menu));
    }

    public async Task<ApiResult<bool>> AssignMenuToRoleAsync(int menuId, int roleId)
    {
        var existing = await _unitOfWork.MenuRolePermissions.FindAsync(p => p.MenuId == menuId && p.RoleId == roleId);
        if (existing.Any()) return ApiResult<bool>.SuccessResult(true);

        var permission = new MenuRolePermission
        {
            MenuId = menuId,
            RoleId = roleId,
            CanView = true
        };

        await _unitOfWork.MenuRolePermissions.AddAsync(permission);
        await _unitOfWork.CompleteAsync();
        return ApiResult<bool>.SuccessResult(true);
    }
}
