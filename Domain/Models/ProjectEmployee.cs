using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Domain.Models
{
    public class ProjectEmployee
    {
        public int Id { get; set; }
        public int ProjectID { get; set; }
        public int EmployeeID { get; set; }
    }
}
