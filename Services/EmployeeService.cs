using Microsoft.EntityFrameworkCore;
using WorkLog.API.Data;
using WorkLog.API.DTOs;
using WorkLog.API.Models;
using WorkLog.API.Services.Interfaces;

namespace WorkLog.API.Services;

// Contains employee business logic and query composition for list endpoints.
public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private const int MaxPageSize = 100;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    // Applies OR-based filtering, sorting, and pagination in a single query pipeline.
    public async Task<PagedResult<Employee>> GetEmployeesAsync(EmployeeFilterDto filter)
    {
        var query = _context.Employees.AsQueryable();

        // Protect paging from invalid query-string values.
        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, MaxPageSize);

        var hasSearch = !string.IsNullOrWhiteSpace(filter.Search);
        var hasRole = !string.IsNullOrWhiteSpace(filter.Role);
        var hasBloodGroup = !string.IsNullOrWhiteSpace(filter.BloodGroup);
        var hasJoinedFrom = filter.JoinedFrom.HasValue;
        var hasJoinedTo = filter.JoinedTo.HasValue;
        var hasJoinedRange = hasJoinedFrom || hasJoinedTo;
        var hasAnyFilter = hasSearch || hasRole || hasBloodGroup || hasJoinedRange;

        // Combine active filter groups with OR.
        // A record is included when it matches any provided group.
        // Joined date is treated as one group; when both bounds are present, both must match.
        if (hasAnyFilter)
        {
            query = query.Where(e =>
                (hasSearch && (e.FirstName.Contains(filter.Search!) ||
                               e.LastName.Contains(filter.Search!) ||
                               e.Email.Contains(filter.Search!))) ||
                (hasRole && e.Role == filter.Role) ||
                (hasBloodGroup && e.BloodGroup == filter.BloodGroup) ||
                (hasJoinedRange &&
                 (!hasJoinedFrom || (e.JoinedDate.HasValue && e.JoinedDate.Value >= filter.JoinedFrom!.Value)) &&
                 (!hasJoinedTo || (e.JoinedDate.HasValue && e.JoinedDate.Value <= filter.JoinedTo!.Value))));
        }

        // Keep sorting constrained to known columns to avoid dynamic-string SQL paths.
        query = filter.SortBy?.ToLower() switch
        {
            "firstname" => filter.Desc ? query.OrderByDescending(e => e.FirstName)
                                       : query.OrderBy(e => e.FirstName),
            "joineddate" => filter.Desc ? query.OrderByDescending(e => e.JoinedDate)
                                        : query.OrderBy(e => e.JoinedDate),
            _ => filter.Desc ? query.OrderByDescending(e => e.Id)
                             : query.OrderBy(e => e.Id)
        };

        var totalRecords = await query.CountAsync();
        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Employee>
        {
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = data
        };
    }

    public async Task<Employee?> GetByIdAsync(int id)
        => await _context.Employees.FindAsync(id);

    public async Task<Employee> CreateAsync(Employee employee)
    {
        // Force create semantics even if the client sends a non-zero Id.
        employee.Id = 0;

        // Normalize creation timestamp in UTC.
        employee.CreatedAt = DateTime.UtcNow;
        employee.UpdatedAt = null;

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> UpdateAsync(int id, Employee employee)
    {
        // Avoid accidental updates when route and payload differ.
        if (id != employee.Id)
            return false;

        employee.UpdatedAt = DateTime.UtcNow;
        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }
}
