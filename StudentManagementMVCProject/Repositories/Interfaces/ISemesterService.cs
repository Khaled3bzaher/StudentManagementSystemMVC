using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface ISemesterService : IGenericRepository<Semester>
    {
        Task<Semester> GetCurrentSemesterAsync();
        Task<bool> CurrentSemesterRegistrationIsOpenedAsync();
        Task<List<Semester>> GetSemestersByAcademicYearAsync(int academicYearId);
    }
}
