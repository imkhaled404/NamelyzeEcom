using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NamelyzeEcom.Business.Interfaces;
using NamelyzeEcom.Business.DTOs;

namespace NamelyzeEcom.Web.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;

        public AdminController(IUserService userService, IMenuService menuService)
        {
            _userService = userService;
            _menuService = menuService;
        }

        [Route("Login")]
        public IActionResult Login() => View();

        [Route("")]
        public IActionResult Root() => RedirectToAction("Dashboard");

        [Route("Dashboard")]
        public IActionResult Dashboard() => View();

        [Route("Users/Create")]
        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await _userService.GetAllRolesAsync();
            ViewBag.Roles = roles.Data;
            return View();
        }

        [Route("Users/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterUserDto dto, int roleId)
        {
            var result = await _userService.RegisterAsync(dto);
            if (result.Success)
            {
                var user = await _userService.GetUserByEmailAsync(dto.Email);
                if (user.Success)
                {
                    var userDto = user.Data!;
                    userDto.RoleId = roleId;
                    await _userService.UpdateUserAsync(userDto.Id, userDto);
                }
                return RedirectToAction("Users");
            }
            ModelState.AddModelError("", result.Message);
            var roles = await _userService.GetAllRolesAsync();
            ViewBag.Roles = roles.Data;
            return View(dto);
        }

        [Route("Products")]
        public IActionResult Products() => View();

        [Route("Products/Create")]
        [HttpGet]
        public IActionResult CreateProduct() => View();

        [Route("Categories")]
        public IActionResult Categories() => View();

        [Route("SubCategories")]
        public IActionResult SubCategories() => View();

        [Route("ChildCategories")]
        public IActionResult ChildCategories() => View();

        [Route("Brands")]
        public IActionResult Brands() => View();

        [Route("Reviews")]
        public IActionResult Reviews() => View();

        [Route("Orders")]
        public IActionResult Orders() => View();

        [Route("OrderTracking")]
        public IActionResult OrderTracking() => View();

        [Route("Customers")]
        public IActionResult Customers() => View();

        [Route("CustomerGroups")]
        public IActionResult CustomerGroups() => View();

        [Route("CustomerDetails/{id?}")]
        public IActionResult CustomerDetails(int? id) => View();

        [Route("Coupons")]
        public IActionResult Coupons() => View();

        [Route("FlashSale")]
        public IActionResult FlashSale() => View();

        [Route("Banners")]
        public IActionResult Banners() => View();

        [Route("Sliders")]
        public IActionResult Sliders() => View("Banners");

        [Route("Blog")]
        public IActionResult Blog() => View();

        [Route("Inventory")]
        public IActionResult Inventory() => View();

        [Route("Stock")]
        public IActionResult Stock() => View("Inventory");

        [Route("Purchase")]
        public IActionResult Purchase() => View("Inventory");

        [Route("Adjustment")]
        public IActionResult Adjustment() => View("Inventory");

        [Route("Reports/Sales")]
        public IActionResult SalesReports() => View();

        [Route("Reports/Inventory")]
        public IActionResult InventoryReports() => View();

        [Route("Reports/Products")]
        public IActionResult ProductReports() => View();

        [Route("Reports/Customers")]
        public IActionResult CustomerReports() => View();

        [Route("Settings")]
        public IActionResult Settings() => View();

        [Route("Users")]
        public IActionResult Users() => View();

        [Route("Roles")]
        public IActionResult Roles() => View();

        [Route("Permissions")]
        public IActionResult Permissions() => View();

        [Route("ActivityLogs")]
        public IActionResult ActivityLogs() => View();

        [HttpPost]
        [Route("Roles/Create")]
        public async Task<IActionResult> CreateRole(string name, string description)
        {
            var role = new NamelyzeEcom.Data.Entities.Role { Name = name, Description = description };
            await _userService.CreateRoleAsync(role);
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("Menus/Create")]
        public async Task<IActionResult> CreateMenu(NamelyzeEcom.Business.DTOs.MenuDto dto)
        {
            await _menuService.CreateMenuAsync(dto);
            return Json(new { success = true });
        }
    }
}
