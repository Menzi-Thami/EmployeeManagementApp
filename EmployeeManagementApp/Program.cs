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
builder.Services.AddRazorPages();

// Register AutoMapper with the MappingProfile
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Retrieve the connection string from configuration
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register the repositories and services with the connection string
builder.Services.AddScoped<IEmployeeRepository>(provider =>
    new EmployeeRepository(connectionString, provider.GetRequiredService<ILogger<EmployeeRepository>>()));
builder.Services.AddScoped<IProjectRepository>(provider =>
    new ProjectRepository(connectionString, provider.GetRequiredService<ILogger<ProjectRepository>>()));
builder.Services.AddScoped<IProjectCostCalculator>(provider =>
    new ProjectCostCalculator(connectionString, provider.GetRequiredService<ILogger<ProjectCostCalculator>>()));
builder.Services.AddScoped<IJobTitleRepository>(provider =>
    new JobTitleRepository(connectionString, provider.GetRequiredService<ILogger<JobTitleRepository>>()));

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBulkInsertService, BulkInsertService>();

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

// Set up custom routing for Add Employee and View Projects.
//app.MapControllerRoute(
//    name: "AddEmployee",
//    pattern: "addemploy",
//    defaults: new { controller = "EmployeeController", action = "AddEmployee" }
//);

//app.MapControllerRoute(
//    name: "ViewProjects",
//    pattern: "viewprojects",
//    defaults: new { controller = "ProjectController", action = "ViewProjects" }
//);

//// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
