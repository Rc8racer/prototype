using Microsoft.EntityFrameworkCore;
using WorkLog.API.Data;
using WorkLog.API.Services;
using WorkLog.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Registers metadata needed for Swagger/OpenAPI docs.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registers EF Core with SQL Server using DefaultConnection from appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enables attribute-routed controllers (e.g., EmployeesController).
builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Maps controller routes like /api/Employees.
app.MapControllers();

// Enable Swagger UI only in Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
