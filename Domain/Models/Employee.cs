using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Domain.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int JobTitleId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string JobTitleName { get; set; }
    }
}
