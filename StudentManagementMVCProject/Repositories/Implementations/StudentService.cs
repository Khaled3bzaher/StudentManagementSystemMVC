using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.DTOs.Students;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Students;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class StudentService : GenericRepository<Student>, IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public StudentService(ApplicationDbContext context,IUnitOfWork unitOfWork,UserManager<User> userManager,IMapper mapper,IFileStorageService fileStorageService) : base(context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }



        public async Task<Student> AddStudentUserAsync(AddStudentDTO model)
        {
            
            var isExistBefore=await _userManager.FindByEmailAsync(model.Email);
            if (isExistBefore != null)
            {
                return null;
            }
            var newUser = _mapper.Map<User>(model);
            var result= await _userManager.CreateAsync(newUser,model.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Student");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return null;
            }
            var newStudent = _mapper.Map<Student>(model);
            newStudent.UserId=newUser.Id;
            await _unitOfWork.Repository<Student>().AddAsync(newStudent);
            await _unitOfWork.CompleteAsync();
            return newStudent;
        }

        public async Task<List<StudentListViewModel>> GetFilteredStudentsListAsync(StudentSearchFilterViewModel filter)
        {
            var StudentsQuery =  _unitOfWork.Repository<Student>().GetAsQueryAble()
                .Include(s => s.User).Include(s => s.Department).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.StudentNumber))
                StudentsQuery = StudentsQuery.Where(s => s.StudentNumber.Contains(filter.StudentNumber));
            if (!string.IsNullOrWhiteSpace(filter.FullName))
                    StudentsQuery = StudentsQuery.Where(s => (s.User.FirstName + " " + s.User.LastName).Contains(filter.FullName));
            if (!string.IsNullOrWhiteSpace(filter.NationalId))
                StudentsQuery = StudentsQuery.Where(s => s.User.NationalId.Contains(filter.NationalId));
            if (!string.IsNullOrWhiteSpace(filter.Email))
                StudentsQuery = StudentsQuery.Where(s => s.User.Email.Contains(filter.Email));
            if (!string.IsNullOrWhiteSpace(filter.Department))
                StudentsQuery = StudentsQuery.Where(s => s.Department.Name.Contains(filter.Department) || s.Department.Code.Contains(filter.Department));
            if (filter.AcademicLevel.HasValue)
                StudentsQuery = StudentsQuery.Where(s => s.AcademicLevel== filter.AcademicLevel.Value);
            if (filter.StudentStatus.HasValue)
                StudentsQuery = StudentsQuery.Where(s => s.Status == filter.StudentStatus.Value);

            return await StudentsQuery.ProjectTo<StudentListViewModel>(_mapper.ConfigurationProvider).ToListAsync();


        }

        public async Task<StudentDetailsViewModel> GetStudentDetailsAsync(int id)
        {
            return await _unitOfWork.Repository<Student>().GetAsQueryAble().Where(s => s.Id==id).Include(s=>s.User).Include(s=>s.Department).ProjectTo<StudentDetailsViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<StudentEditViewModel?> GetStudentForEditAsync(int id)
        {
            return await _unitOfWork.Repository<Student>().GetAsQueryAble().Where(s => s.Id == id).Include(s => s.User).Include(s => s.Department).ProjectTo<StudentEditViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<int> GetStudentIdByUserAsync(string name)
        {
            var student=await _unitOfWork.Repository<Student>().GetAsQueryAble().FirstOrDefaultAsync(s=>s.User.UserName==name);
            return student.Id;
        }

        public async Task<List<StudentListViewModel>> GetStudentsListAsync()
        {
            return await _unitOfWork.Repository<Student>().GetAsQueryAble()
                .Include(s => s.User).Include(s => s.Department)
                .ProjectTo<StudentListViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<StudentCoursesViewModel?> GetStudentCoursesBySemesterIdAsync(int studentId,int semesterId)
        {
            var student = await GetStudentDetailsAsync(studentId);
            if (student == null)
                return null;

            var StudentEnrollments = await _unitOfWork.Repository<Enrollment>().GetAsQueryAble()
                .Include(e => e.CourseSchedule).ThenInclude(cs => cs.Course)
                .Include(e=>e.Grade)
                .Where(e => e.CourseSchedule.SemesterId == semesterId && e.StudentId==studentId)
                .ToListAsync();

            var viewModel = new StudentCoursesViewModel
            {
                StudentId = studentId,
                StudentName = student.FullName,
                CoursesEnrollment = StudentEnrollments.Select(se => new StudentCourseEnrollment
                {
                    CourseCode = se.CourseSchedule.Course.Code,
                    CourseId = se.CourseSchedule.CourseId,
                    CourseName = se.CourseSchedule.Course.Name,
                    TotalGrade = se.Grade?.TotalGrade ?? 0,
                    Assignments = se.Grade?.Assignments ?? 0,
                    AttendanceGrade = se.Grade?.AttendanceGrade ?? 0,
                    Final = se.Grade?.Final ?? 0,
                    MidTerm = se.Grade?.MidTerm ?? 0,
                    Projects= se.Grade?.Projects ?? 0,
                    LetterGrade = se.Grade?.LetterGrade ?? "F"
                }).ToList()
            };

            return viewModel;
        }
        public async Task<(bool Success, string ErrorMessage)> UpdateStudentAsync(StudentEditViewModel model)
        {
            var student= await _unitOfWork.Repository<Student>().GetAsQueryAble().Include(s=>s.User).FirstOrDefaultAsync(s=>s.Id==model.Id);
            if (student == null)
                return (false, "Student Not Found");
            _mapper.Map(model, student);
            _mapper.Map(model, student.User);
            if (model.NewProfilePicture != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(student.User.ProfilePictureURL))
                    {
                        await _fileStorageService.DeleteImageAsync(student.User.ProfilePictureURL);
                    }

                    // رفع الصورة الجديدة
                    student.User.ProfilePictureURL = await _fileStorageService.UploadImageAsync(model.NewProfilePicture);
                }
                catch (Exception ex)
                {
                    return (false, $"Failed to upload profile picture: {ex.Message}");
                }
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (model.NewPassword != model.ConfirmPassword)
                    return (false, "Password and confirmation do not match.");

                var token = await _userManager.GeneratePasswordResetTokenAsync(student.User);
                var result = await _userManager.ResetPasswordAsync(student.User, token, model.NewPassword);
                if (!result.Succeeded)
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            try
            {
                await _unitOfWork.Repository<Student>().UpdateAsync(student);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update student: {ex.Message}");
            }
        }
    }
}
