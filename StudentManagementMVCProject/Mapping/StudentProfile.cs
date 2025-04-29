using AutoMapper;
using StudentManagementMVCProject.DTOs.Students;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.ViewModels.Students;

namespace StudentManagementMVCProject.Mapping
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<AddStudentDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<AddStudentDTO, Student>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest=>dest.AcademicLevel,opt=>opt.MapFrom(src=>src.Level));

            CreateMap<Student,StudentListViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.User.NationalId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Code))
                .ForMember(dest => dest.AcademicLevel, opt => opt.MapFrom(src => (AcademicLevel)src.AcademicLevel))
                .ForMember(dest => dest.StudentStatus, opt => opt.MapFrom(src => (StudentStatus)src.Status))
                ;

            CreateMap<Student,StudentDetailsViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.User.NationalId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => (AcademicLevel)src.AcademicLevel))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StudentStatus)src.Status))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
                .ForMember(dest => dest.isActive, opt => opt.MapFrom(src => src.User.IsActive))
                .ForMember(dest => dest.ProfilePictureURL, opt => opt.MapFrom(src => src.User.ProfilePictureURL))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.ParentNumber, opt => opt.MapFrom(src => src.ParentPhone))

                ;
            CreateMap<Student, StudentEditViewModel>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.User.NationalId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.AcademicLevel, opt => opt.MapFrom(src => (AcademicLevel)src.AcademicLevel))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StudentStatus)src.Status))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive))
            .ForMember(dest => dest.ProfilePictureURL, opt => opt.MapFrom(src => src.User.ProfilePictureURL))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore()) // Prevent modifying User during Student mapping
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Prevent modifying Student.Id
            .ForMember(dest => dest.UserId, opt => opt.Ignore()); // Prevent modifying Student.Id

            // Mapping: StudentEditViewModel -> User
            CreateMap<StudentEditViewModel, User>()
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
