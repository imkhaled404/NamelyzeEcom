using NamelyzeEcom.Business.DTOs;
using NamelyzeEcom.Common.Utilities;

namespace NamelyzeEcom.Business.Interfaces;

public interface IMenuService
{
    Task<ApiResult<IEnumerable<MenuDto>>> GetMenusByRoleAsync(int roleId);
    Task<ApiResult<MenuDto>> CreateMenuAsync(MenuDto dto);
    Task<ApiResult<bool>> AssignMenuToRoleAsync(int menuId, int roleId);
}
