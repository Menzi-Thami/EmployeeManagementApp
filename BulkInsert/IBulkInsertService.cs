using EmployeeManagementApp.Domain.Models;

public interface IBulkInsertService
{
    Task FetchAndBulkInsertProjectLocationsAsync(); 
    Task BulkInsertProjectLocationsAsync(List<ProjectLocations> locations); 
}
