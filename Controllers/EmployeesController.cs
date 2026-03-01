using Microsoft.AspNetCore.Mvc;
using WorkLog.API.DTOs;
using WorkLog.API.Models;
using WorkLog.API.Services.Interfaces;

namespace WorkLog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // Returns a paged employee list with filter and sort options from query string.
    [HttpGet]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] string? bloodGroup,
        [FromQuery] DateTime? joinedFrom,
        [FromQuery] DateTime? joinedTo,
        [FromQuery] string? sortBy = "Id",
        [FromQuery] bool desc = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new EmployeeFilterDto
        {
            Search = search,
            Role = role,
            BloodGroup = bloodGroup,
            JoinedFrom = joinedFrom,
            JoinedTo = joinedTo,
            SortBy = sortBy,
            Desc = desc,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _employeeService.GetEmployeesAsync(filter);
        return Ok(result);
    }

    // Returns a single employee by primary key.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    // Creates an employee and returns its resource location.
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(Employee employee)
    {
        var created = await _employeeService.CreateAsync(employee);
        return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
    }

    // Replaces an existing employee by id.
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
    {
        var success = await _employeeService.UpdateAsync(id, employee);
        if (!success)
            return BadRequest();

        return NoContent();
    }

    // Deletes an employee by id.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var success = await _employeeService.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
