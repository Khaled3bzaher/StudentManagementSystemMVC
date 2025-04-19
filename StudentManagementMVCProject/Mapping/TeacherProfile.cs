using AutoMapper;
using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Mapping
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<AddTeacherDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<AddTeacherDTO, Teacher>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Teacher,TeacherListViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.User.NationalId))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                ;

            CreateMap<Teacher, TeacherNameViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

        }
    }
}
