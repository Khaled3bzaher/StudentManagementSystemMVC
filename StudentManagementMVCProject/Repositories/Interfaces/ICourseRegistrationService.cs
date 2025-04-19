using StudentManagementMVCProject.ViewModels.Courses;
using StudentManagementMVCProject.ViewModels.Registration;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface ICourseRegistrationService
    {
        Task<RegistrationResult> GetRegistrationDetailsAsync(int studentId);
        Task<RegistrationResult> RegisterCoursesAsync(int studentId, List<int> courseIds);
        Task<int> CalculateTotalCreditHoursAsync(List<int> courseIds);
        Task UpdateAcademicRecordAsync(int studentId, int semesterId, int addedCredits,int addCourses);
    }
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public RegistrationViewModel Data { get; set; }
        public List<CourseViewModel> RegisteredCourses { get; set; }
    }
}
