using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.ViewModels.Courses;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface ICourseService : IGenericRepository<Course>
    {
        Task<IEnumerable<CourseViewModel>> GetAllCoursesAsyncOptimized();
        Task<CourseViewModel> GetCourseByIdAsyncOptimized(int id);

        Task<IEnumerable<CourseDropDownItemsViewModel>> GetCoursesDropDownListItemsAsync();
        Task<IEnumerable<CourseDropDownItemsViewModel>> GetCoursesDropDownListItemsExcludeCurrentCourseAsync(int id);

        Task<Course?> GetCourseByIdAsync(int id);
        Task<bool> HasPassedCourse(int id,int CourseId);
        Task<bool> CheckPrerequisitesAsync(int id, int CourseId);
        Task<List<Course>> GetAvaliableCoursesWithSemesterAsync(int studentId, int SemesterId);
        Task<CourseViewModel> GetCourseDependent(int id);
    }
}
