using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface IAcademicRecordService : IGenericRepository<AcademicRecord>
    {
        Task<AcademicRecord?> GetStudentLastAcademicRecordAsync(int studentId);
        Task<AcademicRecord?> GetAcademicRecordByStudentAndSemesterAsync(int studentId, int semesterId);
        Task<List<AcademicRecord>> GetAllAcademicRecordsStudentHistoryAsync(int studentId);
    }
}
