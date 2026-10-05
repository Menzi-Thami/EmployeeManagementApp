using EmployeeManagementApp.Domain.Models;

public interface IBulkInsertService
{
    Task FetchAndBulkInsertProjectLocationsAsync(CancellationToken cancellationToken);
    Task BulkInsertProjectLocationsAsync(List<ProjectLocations> locations, CancellationToken cancellationToken);
}
