namespace WorkLog.API.DTOs;

// Query-string DTO for employee list filtering, sorting, and paging.
public class EmployeeFilterDto
{
    // Free-text search across first name, last name, and email.
    public string? Search { get; set; }
    // Exact match on employee role.
    public string? Role { get; set; }
    // Exact match on blood group.
    public string? BloodGroup { get; set; }

    // Optional joined date lower bound (inclusive).
    public DateTime? JoinedFrom { get; set; }
    // Optional joined date upper bound (inclusive).
    public DateTime? JoinedTo { get; set; }

    // Supported values in service: id, firstname, joineddate.
    public string? SortBy { get; set; } = "Id";
    // False = ascending, true = descending.
    public bool Desc { get; set; } = false;

    // 1-based page number.
    public int PageNumber { get; set; } = 1;
    // Records per page.
    public int PageSize { get; set; } = 10;
}
