using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface ITeacherService : IGenericRepository<Teacher>
    {
        Task<Teacher> AddTeacherUserAsync(AddTeacherDTO model);
        Task<IEnumerable<TeacherNameViewModel>> GetTeachersNameAsync();

    }
}
