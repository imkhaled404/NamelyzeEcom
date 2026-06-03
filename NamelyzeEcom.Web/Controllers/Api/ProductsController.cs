using Microsoft.AspNetCore.Mvc;
using NamelyzeEcom.Business.DTOs;
using NamelyzeEcom.Business.Interfaces;

namespace NamelyzeEcom.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetAllProductsAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetProductBySlug(string slug)
    {
        var result = await _productService.GetProductBySlugAsync(slug);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProducts()
    {
        var result = await _productService.GetFeaturedProductsAsync();
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var result = await _productService.SearchProductsAsync(q);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        var result = await _productService.CreateProductAsync(dto);
        return Ok(result);
    }

    [HttpPost("bulk-upload")]
    public async Task<IActionResult> BulkUpload(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty");
        
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var result = await _productService.BulkUploadAsync(ms.ToArray());
        return Ok(result);
    }

    [HttpPost("track")]
    public async Task<IActionResult> TrackActivity([FromQuery] int productId, [FromQuery] string type)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            await _productService.TrackActivityAsync(userId, productId, type);
        }
        return Ok();
    }
}
