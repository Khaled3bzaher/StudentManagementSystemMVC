using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Courses;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class CourseService : GenericRepository<Course>, ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(ApplicationDbContext context , IUnitOfWork unitOfWork,IMapper mapper) : base(context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CourseViewModel> GetCourseDependent(int id)
        {
            return await _unitOfWork.Repository<Course>().GetAsQueryAble().Where(c => c.PrerequisiteCourseId == id).ProjectTo<CourseViewModel>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();

        }

        public async Task<IEnumerable<CourseViewModel>> GetAllCoursesAsyncOptimized()
        {
            return await _unitOfWork.Repository<Course>().GetAsQueryAble().ProjectTo<CourseViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<CourseViewModel> GetCourseByIdAsyncOptimized(int id)
        {
            return await _unitOfWork.Repository<Course>().GetAsQueryAble().Where(c=>c.Id==id).ProjectTo<CourseViewModel>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<CourseDropDownItemsViewModel>> GetCoursesDropDownListItemsAsync()
        {
            return await _unitOfWork.Repository<Course>().GetAsQueryAble().ProjectTo<CourseDropDownItemsViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<IEnumerable<CourseDropDownItemsViewModel>> GetCoursesDropDownListItemsExcludeCurrentCourseAsync(int id)
        {
            return await _unitOfWork.Repository<Course>().GetAsQueryAble().Where(course => course.Id != id).ProjectTo<CourseDropDownItemsViewModel>(_mapper.ConfigurationProvider).ToListAsync();

        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Course>().GetAsQueryAble().Include(c => c.Department).Include(c => c.PrerequisiteCourse).Include(c => c.CourseSchedules).ThenInclude(cs => cs.Teacher).SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> HasPassedCourse(int id, int CourseId)
        {
            return await _unitOfWork.Repository<Enrollment>().GetAsQueryAble().AnyAsync(e => e.StudentId == id && e.CourseSchedule.CourseId == CourseId && e.GradeStatus == "Passed");
        }


        public async Task<List<Course>> GetAvaliableCoursesWithSemesterAsync(int studentId, int SemesterId)
        {
            var studentDepartmentId = _unitOfWork.Repository<Student>().GetByIdAsync(studentId).Result.DepartmentId;
            var passedCoursesIds = await _unitOfWork.Repository<Enrollment>()
                .GetAsQueryAble()
                .Where(e => e.StudentId == studentId && e.GradeStatus == "Passed")
                .Select(e => e.CourseSchedule.CourseId)
                .Distinct()
                .ToListAsync() ?? new List<int>();


            var availableCourses = await _unitOfWork.Repository<Course>()
                .GetAsQueryAble()
                .AsNoTracking()
                .Include(c => c.Department)
                .Include(c => c.PrerequisiteCourse)
                .Include(c => c.CourseSchedules)
                    .ThenInclude(cs => cs.Teacher)
                .Where(c => c.isActive &&
                            c.CourseSchedules.Any(cs => cs.SemesterId == SemesterId) &&
                            !passedCoursesIds.Contains(c.Id) &&
                            (c.PrerequisiteCourseId == null || passedCoursesIds.Contains(c.PrerequisiteCourseId.Value)) &&
                            (c.DepartmentId == studentDepartmentId || c.isCommon)
                            )
                .ToListAsync();

            return availableCourses;
        }

        public async Task<bool> CheckPrerequisitesAsync(int id, int CourseId)
        {
            var course = await _unitOfWork.Repository<Course>()
                .GetAsQueryAble()
                .Include(c => c.PrerequisiteCourse)
                .FirstOrDefaultAsync(c => c.Id == CourseId);

            if (course?.PrerequisiteCourseId == null) return true;

            return await _unitOfWork.Repository<Enrollment>()
                .GetAsQueryAble()
                .AnyAsync(e => e.StudentId == id &&
                             e.CourseSchedule.CourseId == course.PrerequisiteCourseId &&
                             e.GradeStatus == "Passed");
        }
    }
}
