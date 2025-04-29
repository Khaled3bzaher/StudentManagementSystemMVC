using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.ViewModels.Courses;
using StudentManagementMVCProject.ViewModels.Students;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface ITeacherService : IGenericRepository<Teacher>
    {
        Task<Teacher> AddTeacherUserAsync(AddTeacherDTO model);
        Task<IEnumerable<TeacherNameViewModel>> GetTeachersNameAsync();
        Task<IEnumerable<TeacherListViewModel>> GetTeachersToListAsync();
        Task<TeacherEditViewModel?> GetTeacherForEditByIdAsync(int id);
        Task<List<TeacherCourseEnrollmentsViewModel>?> GetTeacherCoursesBySemesterIdAsync(int teacherId, int semesterId);
        Task<(bool Success, string ErrorMessage)> UpdateTeacherAsync(TeacherEditViewModel model);
        Task<(bool Success, string ErrorMessage)> DeleteTeacherAsync(int id);
        Task<TeacherDetailsViewModel?> GetTeacherDetailsByIdAsync(int id);
        Task<Teacher?> GetTeacherIdByUserIdAsync(string userId);
    }
}
