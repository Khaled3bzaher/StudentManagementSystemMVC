using AutoMapper;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.ViewModels.Courses;

namespace StudentManagementMVCProject.Mapping
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseViewModel>()
                .ForMember(src => src.Department, opt => opt.MapFrom(src=>src.Department.Code))
                .ForMember(src => src.PrerequisiteCourse, opt => opt.MapFrom(src=>src.PrerequisiteCourse.Name))
                ;


            CreateMap<Course, CourseDropDownItemsViewModel>();
            CreateMap<Course, CourseDropDownItemsViewModel>().ReverseMap();

            
        }
    }
}
