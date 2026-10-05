using System.Data.Common;

namespace EmployeeManagementApp.Application.Common.Interfaces
{
    /// <summary>Creates (unopened) connections to the application database.</summary>
    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection();
    }
}
