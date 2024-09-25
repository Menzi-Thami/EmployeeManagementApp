using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Domain.Models
{
    namespace EmployeeManagementApp.Domain.Models
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Role { get; set; }
            public bool Active { get; set; }
        }
    }
}
