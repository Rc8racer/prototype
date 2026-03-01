using Microsoft.EntityFrameworkCore;
using WorkLog.API.Models;

namespace WorkLog.API.Data;

// EF Core database context: each DbSet maps to a database table.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Maps to Employees table.
    public DbSet<Employee> Employees => Set<Employee>();

    // Maps to Products table.
    public DbSet<Product> Products => Set<Product>();
}
