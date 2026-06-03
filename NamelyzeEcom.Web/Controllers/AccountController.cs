using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NamelyzeEcom.Business.DTOs;
using NamelyzeEcom.Business.Interfaces;
using System.Security.Claims;

namespace NamelyzeEcom.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError("", "Invalid credentials. Please verify your email/phone and password.");
                return View(dto);
            }

            var user = result.Data!;
            // Map RoleId to role name since navigation property may not be loaded
            var roleName = user.RoleId switch
            {
                1 => "SuperAdmin",
                2 => "Admin",
                3 => "Manager",
                4 => "Customer",
                5 => "CustomerSupport",
                6 => "InventoryManager",
                7 => "DeliveryMan",
                _ => user.RoleName ?? "Customer"
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("PasswordHash", user.PasswordHash ?? "")
            };


            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.RoleName != "Customer")
            {
                return RedirectToAction("Index", "Admin");
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _userService.RegisterAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userResult = await _userService.GetUserByIdAsync(userId);
            
            if (!userResult.Success) return RedirectToAction("Login");

            // Simplified profile for now, can be expanded with addresses/orders later
            var profile = new UserProfileDto
            {
                User = userResult.Data!,
                TotalOrders = 0 // Placeholder
            };

            return View(profile);
        }
    }
}
