using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EmployeeManagementApp.Domain.Models;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;

namespace EmployeeManagementConsoleApp.Services
{
    public class BulkInsertService : IBulkInsertService
    {
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CodeWorks;Trusted_Connection=True;MultipleActiveResultSets=true"; // Hardcoded connection string
        private readonly ILogger<BulkInsertService> _logger;
        private readonly HttpClient _httpClient;

        public BulkInsertService(ILogger<BulkInsertService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        }

        public async Task FetchAndBulkInsertProjectLocationsAsync()
        {
            string url = "https://www.webafrica.co.za/includes/fibregeolocation.handler.php?cmd=sources&polygon=1";
            var response = await _httpClient.GetStringAsync(url);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response);

            if (apiResponse?.Data != null)
            {
                var locations = apiResponse.Data.Select(d => new ProjectLocations
                {
                    Id = d.Id,
                    Name = d.Name,
                    Location = d.Location
                }).ToList();

                await BulkInsertProjectLocationsAsync(locations);
            }
            else
            {
                _logger.LogWarning("No data found in API response.");
            }
        }

        public async Task BulkInsertProjectLocationsAsync(List<ProjectLocations> locations)
        {
            _logger.LogInformation("Starting bulk insert of project locations...");

            var stopwatch = Stopwatch.StartNew();
            int batchSize = 1000;

            try
            {
                var batchedData = SplitList(locations, batchSize);

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    foreach (var batch in batchedData)
                    {
                        using (var bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "ProjectLocations";
                            var dataTable = ConvertToDataTable(batch);

                            // Perform the bulk copy
                            await bulkCopy.WriteToServerAsync(dataTable);
                            _logger.LogInformation($"Inserted batch of {batch.Count} records.");
                        }
                    }
                }

                stopwatch.Stop();
                _logger.LogInformation($"Bulk insert completed successfully in {stopwatch.Elapsed.TotalSeconds} seconds.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Error occurred during bulk insert after {stopwatch.Elapsed.TotalSeconds} seconds.");
                throw;
            }
        }

        private static List<List<ProjectLocations>> SplitList(List<ProjectLocations> locations, int batchSize)
        {
            var batches = new List<List<ProjectLocations>>();
            for (int i = 0; i < locations.Count; i += batchSize)
            {
                batches.Add(locations.GetRange(i, Math.Min(batchSize, locations.Count - i)));
            }
            return batches;
        }

        private static DataTable ConvertToDataTable(List<ProjectLocations> locations)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Location", typeof(string));

            foreach (var loc in locations)
            {
                table.Rows.Add(loc.Id, loc.Name, loc.Location);
            }

            return table;
        }
    }
}
