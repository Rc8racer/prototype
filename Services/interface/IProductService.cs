using WorkLog.API.DTOs;
using WorkLog.API.Models;

namespace WorkLog.API.Services.Interfaces;

// Contract definition for Product business logic
public interface IProductService
{
    Task<PagedResult<Product>> GetProductsAsync(ProductFilterDto filter);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
}