using Microsoft.EntityFrameworkCore;

namespace WorkLog.API.Models;

public class Product
{
    public int Id { get; set; }

    // Basic Details
    public string Code { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    // Pricing
    [Precision(18, 2)]
    public decimal Price { get; set; }

    // Audit Fields
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    // Status
    public bool IsAvailable { get; set; } = true;
}
