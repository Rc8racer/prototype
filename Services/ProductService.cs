using Microsoft.EntityFrameworkCore;
using WorkLog.API.Data;
using WorkLog.API.DTOs;
using WorkLog.API.Models;
using WorkLog.API.Services.Interfaces;

namespace WorkLog.API.Services;

// Contains product business logic and query composition for list endpoints.
public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private const int MaxPageSize = 100;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // Applies OR-based filtering, sorting, and pagination in a single query pipeline.
    public async Task<PagedResult<Product>> GetProductsAsync(ProductFilterDto filter)
    {
        var query = _context.Products.AsQueryable();

        // Protect paging from invalid query-string values.
        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, MaxPageSize);

        var hasSearch = !string.IsNullOrWhiteSpace(filter.Search);
        var hasBrand = !string.IsNullOrWhiteSpace(filter.Brand);
        var hasCategory = !string.IsNullOrWhiteSpace(filter.Category);
        var hasMinPrice = filter.MinPrice.HasValue;
        var hasMaxPrice = filter.MaxPrice.HasValue;
        var hasPrice = hasMinPrice || hasMaxPrice;
        var hasAvailability = filter.IsAvailable.HasValue;
        var hasAnyFilter = hasSearch || hasBrand || hasCategory || hasPrice || hasAvailability;

        // Combine active filter groups with OR.
        // A record is included when it matches any provided group.
        // Price is treated as one group; when both bounds are present, both must match.
        if (hasAnyFilter)
        {
            query = query.Where(p =>
                (hasSearch && (p.ProductName.Contains(filter.Search!) || p.Code.Contains(filter.Search!))) ||
                (hasBrand && p.BrandName == filter.Brand) ||
                (hasCategory && p.Category == filter.Category) ||
                (hasPrice && (!hasMinPrice || p.Price >= filter.MinPrice!.Value) &&
                             (!hasMaxPrice || p.Price <= filter.MaxPrice!.Value)) ||
                (hasAvailability && p.IsAvailable == filter.IsAvailable));
        }

        // Keep sorting constrained to known columns to avoid dynamic-string SQL paths.
        query = filter.SortBy?.ToLower() switch
        {
            "price" => filter.Desc ? query.OrderByDescending(p => p.Price)
                                   : query.OrderBy(p => p.Price),

            "name" => filter.Desc ? query.OrderByDescending(p => p.ProductName)
                                  : query.OrderBy(p => p.ProductName),

            _ => filter.Desc ? query.OrderByDescending(p => p.Id)
                             : query.OrderBy(p => p.Id)
        };

        var totalRecords = await query.CountAsync();

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = data
        };
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.FindAsync(id);

    public async Task<Product> CreateAsync(Product product)
    {
        // Force create semantics even if the client sends a non-zero Id.
        product.Id = 0;

        // Normalize creation timestamp in UTC.
        product.CreatedOn = DateTime.UtcNow;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(int id, Product product)
    {
        // Avoid accidental updates when route and payload differ.
        if (id != product.Id)
            return false;

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
