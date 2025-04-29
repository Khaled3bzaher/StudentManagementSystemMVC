using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class AcademicRecordService : GenericRepository<AcademicRecord>, IAcademicRecordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AcademicRecordService(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AcademicRecord?> GetAcademicRecordByStudentAndSemesterAsync(int studentId, int semesterId)
        {
            return await _unitOfWork.Repository<AcademicRecord>().GetAsQueryAble().FirstOrDefaultAsync(r=>r.StudentId==studentId&&r.SemesterId==semesterId);
        }

        public async Task<List<AcademicRecord>> GetAllAcademicRecordsStudentHistoryAsync(int studentId)
        {
            return await _unitOfWork.Repository<AcademicRecord>().GetAsQueryAble()
                .Where(r=>r.StudentId == studentId)
                .Include(r=>r.Semester)
                .OrderByDescending(r=>r.Semester.StartDate)
                .ToListAsync();
        }

        

        public async Task<AcademicRecord?> GetStudentLastAcademicRecordAsync(int studentId)
        {
            return await _unitOfWork.Repository<AcademicRecord>().GetAsQueryAble().Where(r=>r.StudentId == studentId).Include(r => r.Semester).OrderByDescending(r=>r.Semester.StartDate).FirstOrDefaultAsync();
        }

        
    }
}
