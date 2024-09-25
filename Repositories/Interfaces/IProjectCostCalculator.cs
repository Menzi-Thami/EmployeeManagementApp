using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Infrastructure.Interfaces
{
    public interface IProjectCostCalculator
    {
        decimal CalculateProjectCost(int projectId);
    }
}