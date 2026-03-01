using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLog.API.Data;
using WorkLog.API.Models;

namespace WorkLog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    // Returns all employee rows from the Employees table.
    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees.ToListAsync();
        return Ok(employees);
    }

    // Creates a new employee row.
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(Employee employee)
    {
        // Set CreatedAt server-side when client doesn't provide a value.
        if (employee.CreatedAt == default)
        {
            employee.CreatedAt = DateTime.UtcNow;
        }

        // New records start with no update timestamp.
        employee.UpdatedAt = null;

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return Ok(employee);
    }
}
