using StudentManagementMVCProject.DTOs.Students;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.ViewModels.Students;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface IStudentService : IGenericRepository<Student>
    {
        Task<int> GetStudentIdByUserAsync(string name);
        Task<Student> AddStudentUserAsync(AddStudentDTO model);
        Task<(List<StudentListViewModel>, int TotalCount)> GetStudentsListAsync(int pageNumber = 1, int pageSize = 4);
        Task<Student> GetStudentIdByUserIdAsync(string userId);

        Task<(List<StudentListViewModel> Items, int TotalCount)> GetFilteredStudentsListAsync(StudentSearchFilterViewModel filter);
        Task<StudentDetailsViewModel> GetStudentDetailsAsync(int id);

        Task<StudentEditViewModel?> GetStudentForEditAsync(int id);
        Task<(bool Success,string ErrorMessage)> UpdateStudentAsync(StudentEditViewModel model);

        Task<StudentCoursesViewModel?> GetStudentCoursesBySemesterIdAsync(int studentId, int semesterId);
    }
}
