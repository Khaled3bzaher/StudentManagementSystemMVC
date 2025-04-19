using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Courses;
using StudentManagementMVCProject.ViewModels.Registration;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class CourseRegistrationService : ICourseRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISemesterService _semesterService;
        private readonly IAcademicRecordService _academicRecordService;
        private readonly ICourseService _courseService;

        public CourseRegistrationService(IUnitOfWork unitOfWork,IMapper mapper,ISemesterService semesterService,IAcademicRecordService academicRecordService , ICourseService courseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _semesterService = semesterService;
            _academicRecordService = academicRecordService;
            _courseService = courseService;
        }
        public async Task<RegistrationResult> GetRegistrationDetailsAsync(int studentId)
        {
            var result = new RegistrationResult();
            var currentSemester = await _semesterService.GetCurrentSemesterAsync();
            var studentStatus = _unitOfWork.Repository<Student>().GetByIdAsync(studentId).Result.Status;
            if (studentStatus != StudentStatus.Active)
            {
                result.Success = false;
                result.Message = $"Contact Administration for Follow Your Status because you are {studentStatus.ToString()}";
                return result;
            }
            if (!await _semesterService.CurrentSemesterRegistrationIsOpenedAsync())
            {
                result.Success = false;
                result.Message = "Course registration is not open at this time.";
                result.Data = new RegistrationViewModel
                {
                    Semester = currentSemester
                };
                return result;
            }
            var lastAcademicRecord= await _academicRecordService.GetStudentLastAcademicRecordAsync(studentId);
            var StudentAcademicRecords = await _academicRecordService.GetAllAcademicRecordsStudentHistoryAsync(studentId);
            
            var avaliableCourses = await _courseService.GetAvaliableCoursesWithSemesterAsync(studentId, currentSemester.Id);
                result.Success = true;
            result.Data = new RegistrationViewModel
            {
                Semester = currentSemester,
                AvailableCourses = _mapper.Map<List<CourseViewModel>>(avaliableCourses),
                AllowedCreditHours = StudentAcademicRecords.Count() == 1 ? 18 : CalculateAllowedHours(lastAcademicRecord),             // عنده سجل اكاديمي واحد او ف نفس الفصل الدراسي لسه طالب جديد

            };
            return result;
        }
        private int CalculateAllowedHours(AcademicRecord academicRecord)
        {
            if (academicRecord == null) return 18;

            if (academicRecord.CumulativeGPA >= 3.0f) return 21;
            if (academicRecord.CumulativeGPA >= 2.0f) return 18;
            return 15;
        }
        public async Task<RegistrationResult> RegisterCoursesAsync(int studentId, List<int> courseIds)
        {
            var result = new RegistrationResult();
            using (var trans = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    
                    if (!await _semesterService.CurrentSemesterRegistrationIsOpenedAsync())
                    {
                        result.Success = false;
                        result.Message = "Course registration is not open at this time.";
                        return result;
                    }
                    var currentSemester = await _semesterService.GetCurrentSemesterAsync();
                    foreach(var courseId in courseIds)
                    {
                        if(!await _courseService.CheckPrerequisitesAsync(studentId, courseId))
                        {
                            var Course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseId);
                            result.Success = false;
                            result.Message = $"You must pass the prerequisite course: {Course.PrerequisiteCourse.Name}";
                            return result;
                        }
                    }
                    var calcHours = await CalculateTotalCreditHoursAsync(courseIds);
                    // A5R Segl Academic LL Student
                    var lastAcademicRecord = await _academicRecordService.GetStudentLastAcademicRecordAsync(studentId);
                    var allowedHours =  CalculateAllowedHours(lastAcademicRecord);
                    if (calcHours > allowedHours)
                    {
                        result.Success = false;
                        result.Message = $"You exceeded the allowed credit hours ({allowedHours} hours)";
                        return result;
                    }
                    //Check LW FEH Registered Before for Current Semester
                    var currentEnrollments = await _unitOfWork.Repository<Enrollment>().GetAsQueryAble().Include(e => e.CourseSchedule).Where(e => e.StudentId == studentId && e.CourseSchedule.SemesterId == currentSemester.Id).ToListAsync();
                    if (currentEnrollments.Count() != 0) {
                        var currentAcademicRecord = await _academicRecordService.GetAcademicRecordByStudentAndSemesterAsync(studentId, currentSemester.Id);
                        await _unitOfWork.Repository<AcademicRecord>().DeleteAsync(currentAcademicRecord);
                        foreach (var enrollment in currentEnrollments)
                        {
                            await _unitOfWork.Repository<Enrollment>().DeleteAsync(enrollment);
                        }
                    }
                    foreach (var courseId in courseIds)
                    {
                        var schedule = await _unitOfWork.Repository<CourseSchedule>().GetAsQueryAble()
                            .FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.SemesterId == currentSemester.Id);
                        
                        if (schedule != null)
                        {
                            var enrollment = new Enrollment
                            {
                                StudentId = studentId,
                                CourseScheduleId = schedule.Id,
                                Status = "Registered",
                                GradeStatus = "Pending"
                            };
                            await _unitOfWork.Repository<Enrollment>().AddAsync(enrollment);
                        }
                    }
                    // A5R Segl Academic LL Student
                    await UpdateAcademicRecordAsync(studentId, currentSemester.Id, calcHours,courseIds.Count());
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    var registeredCourses = await _unitOfWork.Repository<Course>()
                        .GetAsQueryAble()
                        .Where(c => courseIds.Contains(c.Id))
                        .ToListAsync();

                    result.Success = true;
                    result.Message = "Registration completed successfully";
                    result.RegisteredCourses = _mapper.Map<List<CourseViewModel>>(registeredCourses);

                }catch(Exception ex)
                {
                    await trans.RollbackAsync();
                    result.Success = false;
                    result.Message = $"Error during registration: {ex.Message}";
                }

            }
            return result;
        }

        public async Task<int> CalculateTotalCreditHoursAsync(List<int> courseIds)
        {
            var courses = await _unitOfWork.Repository<Course>()
                .GetAsQueryAble()
                .Where(c => courseIds.Contains(c.Id))
                .ToListAsync();

            return courses.Sum(c => c.CreditHour);
        }

        public async Task UpdateAcademicRecordAsync(int studentId, int semesterId, int addedCredits,int addCourses)
        {
            var academicRecord = await _unitOfWork.Repository<AcademicRecord>().GetAsQueryAble()
                .FirstOrDefaultAsync(ar => ar.StudentId == studentId && ar.SemesterId == semesterId);

            if (academicRecord == null)
            {
                academicRecord = new AcademicRecord
                {
                    StudentId = studentId,
                    SemesterId = semesterId,
                    TotalCredits = addedCredits,
                    TotalCourses = addCourses, // سيتم تحديثه لاحقاً
                    SemesterGPA = 0, // سيتم تحديثه لاحقاً
                    CumulativeGPA = 0, // سيتم تحديثه لاحقاً
                    AcademicStanding = "New"
                };
                await _unitOfWork.Repository<AcademicRecord>().AddAsync(academicRecord);
            }
            else
            {
                academicRecord.TotalCredits += addedCredits;
                academicRecord.TotalCourses += addCourses;
                await _unitOfWork.Repository<AcademicRecord>().UpdateAsync(academicRecord);
            }

        }
    }
}
