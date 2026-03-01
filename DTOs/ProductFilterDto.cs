namespace WorkLog.API.DTOs;

// Query-string DTO for product list filtering, sorting, and paging.
public class ProductFilterDto
{
    // Free-text search across ProductName and Code.
    public string? Search { get; set; }
    // Exact match on BrandName.
    public string? Brand { get; set; }
    // Exact match on Category.
    public string? Category { get; set; }

    // Optional lower price bound (inclusive).
    public decimal? MinPrice { get; set; }
    // Optional upper price bound (inclusive).
    public decimal? MaxPrice { get; set; }

    // Optional availability filter.
    public bool? IsAvailable { get; set; }

    // Supported values in service: id, price, name.
    public string? SortBy { get; set; } = "Id";
    // False = ascending, true = descending.
    public bool Desc { get; set; } = false;

    // 1-based page number.
    public int PageNumber { get; set; } = 1;
    // Records per page.
    public int PageSize { get; set; } = 10;
}
