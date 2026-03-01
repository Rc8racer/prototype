using WorkLog.API.DTOs;
using WorkLog.API.Models;

namespace WorkLog.API.Services.Interfaces;

// Contract definition for Employee business logic.
public interface IEmployeeService
{
    Task<PagedResult<Employee>> GetEmployeesAsync(EmployeeFilterDto filter);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> CreateAsync(Employee employee);
    Task<bool> UpdateAsync(int id, Employee employee);
    Task<bool> DeleteAsync(int id);
}
