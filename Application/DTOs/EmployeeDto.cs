using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Application.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class EmployeeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Job Title is required")]
        [Range(1, 4, ErrorMessage = "Job Title ID must be between 1 and 4")]
        public int JobTitleId { get; set; }

        public string JobTitleName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
