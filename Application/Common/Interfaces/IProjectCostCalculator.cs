using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Application.Common.Interfaces
{
    /// <summary>The single source of the project pricing rule (priced by job title, in SQL).</summary>
    public interface IProjectCostCalculator
    {
        Task<decimal> CalculateProjectCostAsync(int projectId, CancellationToken cancellationToken);
    }
}
