using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

public class ProjectEmployeeDto
{
    [Required]
    public int JobTitleId { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Surname cannot exceed 50 characters.")]
    public string Surname { get; set; }
}
