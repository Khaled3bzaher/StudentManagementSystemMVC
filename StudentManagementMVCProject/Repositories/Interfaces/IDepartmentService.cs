using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.ViewModels.Departments;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface IDepartmentService : IGenericRepository<Department>
    {
        //Task<Teacher> GetDepartmentHead(int id);
        Task<IEnumerable<DepartmentViewModel>> GetDepartmentWithHeadsAsync();
        Task <DepartmentViewModel> GetDepartmentByIdAsync(int? id);
        Task<EditDepartmentViewModel> GetDepartmentByIdForEditAsync(int? id);
        Task<IEnumerable<TeacherNameViewModel>> GetDepartmentTeachersByIdAsync(int? id);

        Task<IEnumerable<DepartmentNameCodeViewModel>> GetDepartmentsNamesCodesAsync();
    }

}
