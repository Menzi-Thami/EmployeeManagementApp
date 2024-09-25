using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.Application.DTOs
{
    public class ProjectDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a non-negative number.")]
        public decimal Cost { get; set; }

        public List<string> EmployeeNames { get; set; }

        public List<ProjectEmployeeDto> Employees { get; set; }
    }
}
