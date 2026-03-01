using Microsoft.AspNetCore.Mvc;
using WorkLog.API.DTOs;
using WorkLog.API.Models;
using WorkLog.API.Services.Interfaces;

namespace WorkLog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // Returns a paged product list with filter and sort options from query string.
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] string? brand,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? isAvailable,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] bool desc = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new ProductFilterDto
        {
            Search = search,
            Brand = brand,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            IsAvailable = isAvailable,
            SortBy = sortBy,
            Desc = desc,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _productService.GetProductsAsync(filter);
        return Ok(result);
    }

    // Returns a single product by primary key.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // Creates a product and returns its resource location.
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        var created = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    // Replaces an existing product by id.
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        var success = await _productService.UpdateAsync(id, product);
        if (!success)
            return BadRequest();

        return NoContent();
    }

    // Deletes a product by id.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await _productService.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
