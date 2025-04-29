using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Courses;
using StudentManagementMVCProject.ViewModels.Students;

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

        public async Task<int> GetCourseTeacherIdBySmesterId(int semesterId,int courseId)
        {
            var teacher = await _unitOfWork.Repository<CourseSchedule>().GetAsQueryAble().FirstOrDefaultAsync(cs => cs.SemesterId == semesterId && cs.CourseId == courseId);
            if (teacher is null)
                return 0;
            return teacher.TeacherId;
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
                             e.Status == "Passed");
        }

        //STATUS
        //REGISTERED - PASSED - FAILED


        public async Task<List<CourseStudentsListViewModel>?> GetCourseStudentsListAsync(int courseId,int semesterId)
        {
            return await _unitOfWork.Repository<Enrollment>().GetAsQueryAble()
                .Include(e => e.Grade)
                .Include(e => e.CourseSchedule)
                .Include(e => e.Student)
                .ThenInclude(s => s.User)
                .Where(e => e.CourseSchedule.CourseId == courseId && e.CourseSchedule.SemesterId == semesterId )
                .Select(e => new CourseStudentsListViewModel
                {
                    StudentId = e.StudentId,
                    StudentName = (e.Student.User.FirstName + " " + e.Student.User.LastName),
                    Email = e.Student.User.Email,
                    PhoneNumber = e.Student.User.PhoneNumber,
                    StudentNumber = e.Student.StudentNumber,
                    AcademicLevel = e.Student.AcademicLevel,
                    Assignments = e.Grade.Assignments ?? 0,
                    AttendanceGrade = e.Grade.AttendanceGrade ?? 0,
                    MidTerm=e.Grade.MidTerm ?? 0,
                    Projects=e.Grade.Projects ?? 0,
                    Final=e.Grade.Final ?? 0,
                    TotalGrade=e.Grade.TotalGrade ?? 0,
                    CourseStatus=e.Status,
                    
                }).ToListAsync();
        }

        public async Task<EditStudentGradesViewModel?> GetStudentGradesAsync(int studentId, int courseId, int semesterId)
        {
            return await _unitOfWork.Repository<Enrollment>().GetAsQueryAble()
                .Include(e => e.Grade)
                .Include(e => e.CourseSchedule)
                .Include(e => e.Student)
                .ThenInclude(s => s.User)
                .Where(e => e.CourseSchedule.CourseId == courseId && e.CourseSchedule.SemesterId == semesterId && e.StudentId == studentId)
                .Select(e => new EditStudentGradesViewModel
                {
                    StudentId = e.StudentId,
                    SemesterId = semesterId,
                    CourseId = e.CourseSchedule.CourseId,
                    Assignments = e.Grade.Assignments ?? 0,
                    AttendanceGrade = e.Grade.AttendanceGrade ?? 0,
                    MidTerm = e.Grade.MidTerm ?? 0,
                    Projects = e.Grade.Projects ?? 0,
                    Final = e.Grade.Final ?? 0,
                    TotalGrade = e.Grade.TotalGrade ?? 0,
                    LetterGrade = e.Grade.LetterGrade ?? "F",
                    CanEdit = e.Status == "Registered"


                }).FirstOrDefaultAsync();
        }

        public async Task<(bool success,string errorMessage)> UpdateStudentGradesAsync(EditStudentGradesViewModel model)
        {
            var studentEnrollment = await _unitOfWork.Repository<Enrollment>().GetAsQueryAble()
                .Include(e => e.Grade)
                .Include(e => e.CourseSchedule)
                .FirstOrDefaultAsync(e => e.StudentId == model.StudentId && e.CourseSchedule.SemesterId == model.SemesterId && e.CourseSchedule.CourseId == model.CourseId);

            if (studentEnrollment is null)
                return (false, "Can't Find Course Enrollment ..!");

            if (studentEnrollment.Status == "Finished")
            {
                return (false, "Cannot update grades for a finished course!");
            }

            model.TotalGrade = CalculateTotalGrade(model);
            if(studentEnrollment.Grade is null)
            {
                await _unitOfWork.Repository<Grade>().AddAsync(new Grade {
                    EnrollmentId= studentEnrollment.Id,
                    Assignments = model.Assignments,
                    AttendanceGrade = model.AttendanceGrade,
                    Final = model.Final,
                    MidTerm = model.MidTerm,
                    Projects = model.Projects,
                    TotalGrade = model.TotalGrade,
                    LetterGrade = CalculateLetterGrade(model.TotalGrade),
                    LastUpdated = DateTime.Now,
                });

            }
            else
            {
                studentEnrollment.Grade.Assignments = model.Assignments;
                studentEnrollment.Grade.AttendanceGrade = model.AttendanceGrade;
                studentEnrollment.Grade.Final = model.Final;
                studentEnrollment.Grade.MidTerm = model.MidTerm;
                studentEnrollment.Grade.Projects = model.Projects;
                studentEnrollment.Grade.TotalGrade = model.TotalGrade;
                studentEnrollment.Grade.LetterGrade = CalculateLetterGrade(model.TotalGrade);
                studentEnrollment.Grade.LastUpdated = DateTime.Now;
            }
            studentEnrollment.GradeStatus = "Registered";
            

            await _unitOfWork.CompleteAsync();
            return (true, "Grade Update Successfully..!");
        }
        private float CalculateTotalGrade(EditStudentGradesViewModel model)
        {
            float total = 0;
            total += (model.MidTerm ?? 0);
            total += (model.Final ?? 0);
            total += (model.Assignments ?? 0) ;
            total += (model.Projects ?? 0);
            total += (model.AttendanceGrade ?? 0);

            return total;
        }
        private string CalculateLetterGrade(float? totalGrade)
        {
            if (!totalGrade.HasValue) return null;

            return totalGrade switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                >= 50 => "E",
                _ => "F"
            };
        }
    }
}
