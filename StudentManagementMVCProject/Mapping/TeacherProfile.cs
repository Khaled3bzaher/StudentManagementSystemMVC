using AutoMapper;
using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.ViewModels.Students;
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
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>  src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.User.NationalId))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                ;

            CreateMap<Teacher, TeacherNameViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>  src.User.FirstName + " " + src.User.LastName));

            CreateMap<Teacher,TeacherDetailsViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.User.NationalId))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.ProfilePictureURL, opt => opt.MapFrom(src => src.User.ProfilePictureURL))
                .ForMember(dest => dest.isActive, opt => opt.MapFrom(src => src.User.IsActive))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
                ;
            CreateMap<Teacher, TeacherEditViewModel>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.User.NationalId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive))
            .ForMember(dest => dest.ProfilePictureURL, opt => opt.MapFrom(src => src.User.ProfilePictureURL))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore()) // Prevent modifying User during Student mapping
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            ;

            CreateMap<TeacherEditViewModel, User>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
               .ForMember(dest => dest.Email, opt => opt.Ignore())
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
               .ForMember(dest => dest.ProfilePictureURL, opt => opt.Condition(src => src.NewProfilePicture != null || !string.IsNullOrEmpty(src.ProfilePictureURL)))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.UserName, opt => opt.Ignore())
               .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
               .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
               .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
               .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
               .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
               .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
               .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
               .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
               .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
               .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
               .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore());
        }
    }
}
