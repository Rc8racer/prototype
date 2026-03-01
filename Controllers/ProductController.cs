using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLog.API.Data;
using WorkLog.API.Models;
using WorkLog.API.Helpers;

namespace WorkLog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // GET: api/products (With Pagination)
    // ============================================================
    [HttpGet]
    public async Task<ActionResult<PagedResult<Product>>> GetProducts(
        [FromQuery] PaginationParams paginationParams)
    {
        var query = _context.Products.AsQueryable();

        var result = await PaginationHelper.CreateAsync(
            query,
            paginationParams.PageNumber,
            paginationParams.PageSize);

        return Ok(result);
    }

    // ============================================================
    // GET: api/products/{id}
    // ============================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
            return NotFound();

        return product;
    }

    // ============================================================
    // POST: api/products
    // ============================================================
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // Prevent identity insert errors if client sends an Id.
        product.Id = 0;

        if (product.CreatedOn == default)
            product.CreatedOn = DateTime.UtcNow;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // ============================================================
    // PUT: api/products/{id}
    // ============================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest();

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // ============================================================
    // DELETE: api/products/{id}
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
