using System.ComponentModel.DataAnnotations;

namespace WorkLog.API.Models;

// Employee entity mapped by EF Core to the Employees table.
public class Employee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Role { get; set; } = string.Empty;

    public DateTime? JoinedDate { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(20)]
    public string? AlternateNumber { get; set; }

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
