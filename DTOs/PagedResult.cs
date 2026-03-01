namespace WorkLog.API.DTOs;

// Standard envelope returned by paged list endpoints.
// Keeps paging metadata and the current page records together.
public class PagedResult<T>
{
    // Total rows matching the filter before paging is applied.
    public int TotalRecords { get; set; }
    // 1-based page number requested by the client.
    public int PageNumber { get; set; }
    // Number of records returned per page.
    public int PageSize { get; set; }

    // Materialized records for the current page.
    public List<T> Data { get; set; } = new();
}
