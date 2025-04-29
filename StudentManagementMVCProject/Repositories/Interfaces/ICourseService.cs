using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.ViewModels.Courses;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface ICourseService : IGenericRepository<Course>
    {
        Task<IEnumerable<CourseViewModel>> GetAllCoursesAsyncOptimized();
        Task<CourseViewModel> GetCourseByIdAsyncOptimized(int id);

        

        Task<int> GetCourseTeacherIdBySmesterId(int semesterId, int courseId);
        Task<EditStudentGradesViewModel?> GetStudentGradesAsync(int studentId, int courseId, int semesterId);

        Task<IEnumerable<CourseDropDownItemsViewModel>> GetCoursesDropDownListItemsAsync();
        Task<IEnumerable<CourseDropDownItemsViewModel>> GetCoursesDropDownListItemsExcludeCurrentCourseAsync(int id);
        Task<List<CourseStudentsListViewModel>?> GetCourseStudentsListAsync(int courseId, int semesterId);
        Task<Course?> GetCourseByIdAsync(int id);
        Task<bool> HasPassedCourse(int id,int CourseId);
        Task<bool> CheckPrerequisitesAsync(int id, int CourseId);
        Task<List<Course>> GetAvaliableCoursesWithSemesterAsync(int studentId, int SemesterId);
        Task<CourseViewModel> GetCourseDependent(int id);
        Task<(bool success, string errorMessage)> UpdateStudentGradesAsync(EditStudentGradesViewModel model);
    }
}
