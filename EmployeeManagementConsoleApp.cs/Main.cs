using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmployeeManagementApp.Infrastructure.Repositories;
using EmployeeManagementApp.Domain.Models;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementConsoleApp
{
    class Program 
    {
        static async Task Main(string[] args)
        {
            // Set up configuration to read appsettings.json
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", "Server=(localdb)\\MSSQLLocalDB;Database=CodeWorks;Trusted_Connection=True;MultipleActiveResultSets=true")
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(configure => configure.AddConsole());

                    // Use IConfiguration to get the connection string
                    var configuration = context.Configuration;
                    var connectionString = configuration.GetConnectionString("DefaultConnection");

                    // Pass the connection string to EmployeeRepository
                    services.AddScoped<IEmployeeRepository>(provider =>
                        new EmployeeRepository(connectionString, provider.GetRequiredService<ILogger<EmployeeRepository>>()));
                })
                .Build();

            await host.StartAsync();

            var employeeService = host.Services.GetRequiredService<IEmployeeRepository>();
            await AddEmployee(employeeService);

            await host.StopAsync();
        }

        private static async Task AddEmployee(IEmployeeRepository employeeRepository)
        {
            Console.WriteLine("Enter employee details:");

            string name;
            while (true)
            {
                Console.Write("Name: ");
                name = Console.ReadLine();
                if (IsValidName(name))
                {
                    break; // Valid name, exit loop
                }
                else
                {
                    Console.WriteLine("Invalid name. Please enter a valid name (letters only).");
                }
            }

            string surname;
            while (true)
            {
                Console.Write("Surname: ");
                surname = Console.ReadLine();
                if (IsValidName(surname))
                {
                    break; 
                }
                else
                {
                    Console.WriteLine("Invalid surname. Please enter a valid surname (letters only).");
                }
            }

            int jobTitleId;
            while (true)
            {
                Console.Write("Job Title ID (must be between 1 and 4): ");
                if (int.TryParse(Console.ReadLine(), out jobTitleId) && jobTitleId >= 1 && jobTitleId <= 4)
                {
                    break; 
                }
                else
                {
                    Console.WriteLine("Invalid Job Title ID. Please enter a number between 1 and 4.");
                }
            }

            DateTime dateOfBirth;
            while (true)
            {
                Console.Write("Date of Birth (dd-MMMM-yyyy): ");
                if (DateTime.TryParse(Console.ReadLine(), out dateOfBirth))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Date of Birth. Please enter a valid date.");
                }
            }

            var employee = new Employee
            {
                Name = name,
                Surname = surname,
                JobTitleId = jobTitleId,
                DateOfBirth = dateOfBirth
            };

            await employeeRepository.AddEmployeeAsync(employee);
            Console.WriteLine("Employee added successfully!");
        }

        private static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, @"^[a-zA-Z\s]+$");
        }
    }
}
