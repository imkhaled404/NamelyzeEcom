using Microsoft.AspNetCore.Mvc;
using NamelyzeEcom.Business.Interfaces;
using System.Text;

namespace NamelyzeEcom.Web.Controllers;

[Route("/")]
[ApiController]
public class SeoController : ControllerBase
{
    private readonly IProductService _productService;

    public SeoController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("sitemap.xml")]
    public async Task<IActionResult> GetSitemap()
    {
        var products = await _productService.GetAllProductsAsync(1, 1000);
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
        sb.AppendLine("<url><loc>https://namelyzeecom.com/</loc><priority>1.0</priority></url>");
        
        foreach (var p in products.Data!)
        {
            sb.AppendLine($"<url><loc>https://namelyzeecom.com/product/{p.Slug}</loc><priority>0.8</priority></url>");
        }
        
        sb.AppendLine("</urlset>");
        return Content(sb.ToString(), "application/xml", Encoding.UTF8);
    }

    [HttpGet("robots.txt")]
    public IActionResult GetRobots()
    {
        var sb = new StringBuilder();
        sb.AppendLine("User-agent: *");
        sb.AppendLine("Allow: /");
        sb.AppendLine("Disallow: /admin/");
        sb.AppendLine("Sitemap: https://namelyzeecom.com/sitemap.xml");
        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }
}
