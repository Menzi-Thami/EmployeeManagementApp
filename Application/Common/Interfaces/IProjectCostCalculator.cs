using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Application.Common.Interfaces
{
    public interface IProjectCostCalculator
    {
        decimal CalculateProjectCost(int projectId);
    }
}