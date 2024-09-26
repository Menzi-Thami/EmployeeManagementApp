using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.Application.DTOs
{
    public class JobTitleDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Job title cannot exceed 50 characters.")]
        public string JobTitleName { get; set; }
    }
}
