using Serilog;
using EmployeeManagementApp.Infrastructure.Repositories;
using EmployeeManagementApp.Infrastructure.Calculators;
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
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// One error shape: typed exceptions -> RFC 9457 ProblemDetails (with traceId) on /api routes.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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
