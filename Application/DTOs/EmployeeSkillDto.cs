using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.Application.DTOs
{
    public class EmployeeSkillDto
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public int SkillID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Skill name cannot exceed 100 characters.")]
        public string SkillName { get; set; }
    }
}
