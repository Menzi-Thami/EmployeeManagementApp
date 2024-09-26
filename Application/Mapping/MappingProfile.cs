using AutoMapper;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Domain.Models;
using System.Linq;

namespace EmployeeManagementApp.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Job Title Mappings
            CreateMap<JobTitle, JobTitleDto>().ReverseMap();

            // Employee Mappings
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.JobTitleName, opt => opt.MapFrom(src => src.JobTitle.JobTitleName)); // Ensure this maps correctly
            CreateMap<EmployeeDto, Employee>();

            // Project Employee Mappings
            CreateMap<ProjectEmployee, ProjectEmployeeDto>().ReverseMap();

            // Project Mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.EmployeeNames, opt => opt.MapFrom(src =>
                    src.ProjectEmployees.Select(pe => $"{pe.Employee.Name} {pe.Employee.Surname}").ToList()))
                .ForMember(dest => dest.JobTitles, opt => opt.MapFrom(src =>
                    src.ProjectEmployees.Select(pe => new JobTitleDto
                    {
                        Id = pe.Employee.JobTitleId,
                        JobTitleName = pe.Employee.JobTitle.JobTitleName // Ensure this is correct
                    }).ToList()))
                .ReverseMap();
        }
    }
}
