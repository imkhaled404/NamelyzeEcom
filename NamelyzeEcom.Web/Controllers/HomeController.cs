using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NamelyzeEcom.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Route("Shop")]
        public IActionResult Shop() => View();

        [Route("Product/{slug}")]
        public IActionResult ProductDetail(string slug) => View();

        [Route("Category/{slug?}")]
        public IActionResult Category(string? slug) => View();

        [Route("Offers")]
        public IActionResult Offers() => View();

        [Route("Track")]
        public IActionResult Track() => View();

        [Authorize]
        [Route("Checkout")]
        public IActionResult Checkout() => View();

    }
}
