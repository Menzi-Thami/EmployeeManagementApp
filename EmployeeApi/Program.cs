using Serilog;
using EmployeeManagementApp.Infrastructure.Repositories;
using EmployeeManagementApp.Infrastructure.Calculators;
using EmployeeManagementApp.Infrastructure.Data;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeApi.ErrorHandling;


var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.
// MVC (unlike Razor Pages) only validates antiforgery tokens when a filter asks it to.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddRazorPages();

// One error shape: typed exceptions -> RFC 9457 ProblemDetails (with traceId) on /api routes.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Validated connection string (fails at boot if missing) + Microsoft.Data.SqlClient connection factory.
builder.Services.AddDatabase();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectCostCalculator, ProjectCostCalculator>();
builder.Services.AddScoped<IJobTitleRepository, JobTitleRepository>();


// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

var app = builder.Build();

// /api errors are written by GlobalExceptionHandler; anything it declines (MVC pages)
// is re-executed as the /Home/Error view.
app.UseExceptionHandler("/Home/Error");

// Structured HTTP request logging (method, path, status, elapsed) through the
// already-configured Serilog pipeline.
app.UseSerilogRequestLogging();

// Send Strict-Transport-Security outside development (dev stays on plain HTTP).
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

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
