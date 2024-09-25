using AutoMapper;
using Serilog;
using EmployeeManagementApp.Infrastructure.Repositories;
using EmployeeManagementApp.Infrastructure.Calculators;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Infrastructure.Interfaces;
using EmployeeManagementApp.Application.Mapping; 
using EmployeeManagementConsoleApp.Services; 

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.
builder.Services.AddControllersWithViews(); 

// Register Razor Pages
builder.Services.AddRazorPages(); 

// Register AutoMapper with the MappingProfile
builder.Services.AddAutoMapper(typeof(MappingProfile)); 

// Register the repositories and services
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectCostCalculator, ProjectCostCalculator>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IJobTitleRepository, JobTitleRepository>();

// Register the BulkInsertService with DI
builder.Services.AddScoped<IBulkInsertService, BulkInsertService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages(); // Ensure Razor Pages are mapped

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
