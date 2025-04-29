using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Students;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class TeacherService : GenericRepository<Teacher>, ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TeacherService(ApplicationDbContext context, IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper,IFileStorageService fileStorageService,RoleManager<IdentityRole> roleManager) : base(context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _roleManager = roleManager;
        }
        public async Task<IEnumerable<TeacherListViewModel>> GetTeachersToListAsync()
        {
            return await GetAsQueryAble()
                .Include(s => s.User).Include(s => s.Department)
                .ProjectTo<TeacherListViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<Teacher> AddTeacherUserAsync(AddTeacherDTO model)
        {
            var isExistBefore = await _userManager.FindByEmailAsync(model.Email);
            if (isExistBefore != null)
            {
                return null;
            }
            var newUser = _mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            if (!await _roleManager.RoleExistsAsync("Teacher"))
            {
                var role = new IdentityRole("Teacher");
                var createdRoleResult = await _roleManager.CreateAsync(role);
                if (!createdRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(newUser);
                    return null;
                }
            }
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Teacher");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return null;
            }
            var newTeacher = _mapper.Map<Teacher>(model);
            newTeacher.UserId = newUser.Id;
            await AddAsync(newTeacher);
            await _unitOfWork.CompleteAsync();
            return newTeacher;
        }

        public async Task<IEnumerable<TeacherNameViewModel>> GetTeachersNameAsync()
        {
            return await GetAsQueryAble()
                .Include(s => s.User)
                .ProjectTo<TeacherNameViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TeacherDetailsViewModel?> GetTeacherDetailsByIdAsync(int id)
        {
            return await GetAsQueryAble().Where(t => t.Id == id).Include(t => t.Department)
                .Include(t => t.User).ProjectTo<TeacherDetailsViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<TeacherEditViewModel?> GetTeacherForEditByIdAsync(int id)
        {
            return await GetAsQueryAble().Where(t => t.Id == id).Include(t => t.Department)
                .Include(t => t.User).ProjectTo<TeacherEditViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateTeacherAsync(TeacherEditViewModel model)
        {
            var teacher = await GetAsQueryAble().Where(t=>t.Id==model.Id).Include(t=>t.User).FirstOrDefaultAsync();
            if (teacher == null) {
                return (false, "Teacher Not Found");
            }
            //IF TEACHER IS DEPARTMENT HEAD
            if(model.DepartmentId != teacher.DepartmentId)
            {
                var teacherDept = await _unitOfWork.Repository<Department>().GetByIdAsync(teacher.DepartmentId);
                if (teacherDept.HeadTeacherID == teacher.Id) { 
                    teacherDept.HeadTeacherID = null;
                    await _unitOfWork.Repository<Department>().UpdateAsync(teacherDept);
                }
            }
            _mapper.Map(model, teacher);
            _mapper.Map(model, teacher.User);
            if (model.NewProfilePicture != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(teacher.User?.ProfilePictureURL))
                    {
                        await _fileStorageService.DeleteImageAsync(teacher.User.ProfilePictureURL);
                    }

                    // رفع الصورة الجديدة
                    teacher.User.ProfilePictureURL = await _fileStorageService.UploadImageAsync(model.NewProfilePicture);
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

                var token = await _userManager.GeneratePasswordResetTokenAsync(teacher.User);
                var result = await _userManager.ResetPasswordAsync(teacher.User, token, model.NewPassword);
                if (!result.Succeeded)
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            try
            {
                await UpdateAsync(teacher);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update student: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteTeacherAsync(int id)
        {
            var teacher = await GetByIdAsync(id);
            if(teacher==null)
                return (false, "Teacher Not Found ..!");
            var teacherUser = await _userManager.FindByIdAsync(teacher.UserId);
            if (teacherUser == null)
                return (false, "User Related to Teacher Not Found ..!");
            using (var trans = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await DeleteAsync(teacher);
                    var result = await _userManager.DeleteAsync(teacherUser);
                    if (!result.Succeeded)
                    {
                        await trans.RollbackAsync();
                        return (false, $"Error through Deleting Teacher User ..!");
                    }
                    await _unitOfWork.CompleteAsync();
                    await trans.CommitAsync();
                    return (true,$"Deleting Teacher {teacherUser.FirstName} {teacherUser.LastName} Successfully ..!");
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    return (false, $"Error through Deleting Teacher {ex.Message} ..!");

                }
            }
        }
        public async Task<List<TeacherCourseEnrollmentsViewModel>?> GetTeacherCoursesBySemesterIdAsync(int teacherId, int semesterId) {

            var teacher = await GetTeacherDetailsByIdAsync(teacherId);
            if(teacher==null)
                return null;
            return await _unitOfWork.Repository<CourseSchedule>()
                .GetAsQueryAble()
                .Include(cs => cs.Course).ThenInclude(c => c.Department)
                .Where(cs => cs.TeacherId == teacherId && cs.SemesterId == semesterId && cs.Course.isActive)
                .Select(c => new TeacherCourseEnrollmentsViewModel
                {
                    CourseId = c.Course.Id,
                    CourseCode=c.Course.Code,
                    CourseName = c.Course.Name,
                    CreditHour=c.Course.CreditHour,
                    Department=c.Course.Department.Name
                }).ToListAsync();

        }
        public async Task<Teacher?> GetTeacherIdByUserIdAsync(string userId)
        {
            var teacher = await GetAsQueryAble().Select(s => new Teacher
            {
                Id = s.Id,
                UserId = s.UserId,
            }).FirstOrDefaultAsync(s => s.UserId == userId);
            return teacher;
        }
    }
}
