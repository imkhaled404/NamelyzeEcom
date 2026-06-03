using Microsoft.AspNetCore.Mvc;
using NamelyzeEcom.Business.Interfaces;
using System.Security.Claims;

namespace NamelyzeEcom.Web.ViewComponents
{
    public class RecommendedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;

        public RecommendedProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int limit = 8)
        {
            var userIdClaim = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Fallback for guests: Featured Products
                var featured = await _productService.GetFeaturedProductsAsync();
                return View(featured.Data!.Take(limit));
            }

            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                var recommendations = await _productService.GetPersonalizedRecommendationsAsync(userId, limit);
                return View(recommendations.Data);
            }

            return View(Enumerable.Empty<NamelyzeEcom.Business.DTOs.ProductDto>());
        }
    }
}
