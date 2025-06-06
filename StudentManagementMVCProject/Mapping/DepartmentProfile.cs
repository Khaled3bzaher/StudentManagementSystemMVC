﻿using AutoMapper;
using StudentManagementMVCProject.DTOs.Departments;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.ViewModels.Departments;

namespace StudentManagementMVCProject.Mapping
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile() {
            CreateMap<Department, DepartmentViewModel>()
                .ForMember(dest => dest.DepartmentHeadName, opt => opt.MapFrom(src => src.HeadTeacher.User.FirstName + " " + src.HeadTeacher.User.LastName))
                .ForMember(dest => dest.CoursesCount, opt => opt.MapFrom(src => src.Courses.Count))
                .ForMember(dest => dest.TeachersCount, opt => opt.MapFrom(src => src.Teachers.Count))
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Students.Count))
                ;

            CreateMap<DepartmentViewModel, Department>();

            CreateMap<Department,DepartmentNameCodeViewModel>();

            CreateMap<AddDepartmentDTO, Department>()
                .ForMember(dest => dest.HeadTeacher, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                ;

            CreateMap<EditDepartmentViewModel,Department >().ReverseMap();
        }
    }
}
