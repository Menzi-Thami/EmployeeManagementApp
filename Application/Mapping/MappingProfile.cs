using AutoMapper;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Domain.Models;

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
                .ForMember(dest => dest.JobTitleName, opt => opt.MapFrom(src => src.JobTitleName));
            CreateMap<EmployeeDto, Employee>();

            // Project Employee Mappings
            CreateMap<ProjectEmployee, ProjectEmployeeDto>().ReverseMap();

            // Project Mappings
            CreateMap<Project, ProjectDto>().ReverseMap();
        }
    }
}
