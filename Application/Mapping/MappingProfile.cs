using AutoMapper;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Domain.Models;

namespace EmployeeManagementApp.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<JobTitle, JobTitleDto>().ReverseMap(); 
            CreateMap<Employee, EmployeeDto>().ReverseMap(); 
            CreateMap<ProjectEmployee, ProjectEmployeeDto>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
                                                             
        }
    }
}