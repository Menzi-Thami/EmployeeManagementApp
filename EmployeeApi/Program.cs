using Serilog;
using EmployeeManagementApp.Infrastructure.Repositories;
using EmployeeManagementApp.Infrastructure.Calculators;
using EmployeeManagementApp.Infrastructure.Data;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeApi.ErrorHandling;
using EmployeeApi.Health;
using EmployeeApi.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

// Serilog stays the logging pipeline. Serilog 4 stamps every event with the current
// Activity's TraceId/SpanId, and the output template prints {TraceId}, so each line
// joins up with the request's spans. ReadFrom.Services picks up any ILogEventSink in DI.
// preserveStaticLogger: each host logs through its own logger instead of the process-wide
// static Log.Logger (nothing here uses it), so in-process test hosts don't swap sinks.
builder.Host.UseSerilog((context, services, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext(),
    preserveStaticLogger: true);

builder.Services.AddObservability(builder.Configuration);

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

// /health/live has no checks (a DB blip must not get the process restarted);
// /health/ready runs everything tagged "ready".
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name, tags: ["ready"], timeout: TimeSpan.FromSeconds(5));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectCostCalculator, ProjectCostCalculator>();
builder.Services.AddScoped<IJobTitleRepository, JobTitleRepository>();


// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

var app = builder.Build();

// Outermost, so the header survives UseExceptionHandler clearing the response.
app.UseTraceIdResponseHeader();

// /api errors are written by GlobalExceptionHandler; anything it declines (MVC pages)
// is re-executed as the /Home/Error view.
app.UseExceptionHandler("/Home/Error");

// Structured HTTP request logging (method, path, status, elapsed) through the
// already-configured Serilog pipeline.
// Probes hit /health every few seconds; keep them out of the Information-level request log.
app.UseSerilogRequestLogging(options =>
{
    // The host's logger, not the (preserved, silent) static Log.Logger.
    options.Logger = app.Services.GetRequiredService<Serilog.ILogger>();
    options.GetLevel = (httpContext, _, exception) =>
        exception is null && httpContext.Request.Path.StartsWithSegments("/health")
            ? LogEventLevel.Verbose
            : httpContext.Response.StatusCode >= 500 || exception is not null
                ? LogEventLevel.Error
                : LogEventLevel.Information;
});

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

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = c => c.Tags.Contains("ready") }).AllowAnonymous();

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
