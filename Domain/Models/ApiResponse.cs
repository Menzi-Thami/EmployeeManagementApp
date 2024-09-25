using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Domain.Models
{
    public class ApiResponse
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public List<ProjectLocations> Data { get; set; }
    }
}
