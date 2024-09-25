using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Domain.Models
{
    public class EmployeeSkill
    {
        public int Id { get; set; }
        public int EmployeeID { get; set; }
        public int SkillID { get; set; }
    }
}