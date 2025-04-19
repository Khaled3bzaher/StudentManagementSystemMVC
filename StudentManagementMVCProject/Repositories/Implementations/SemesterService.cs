using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class SemesterService : GenericRepository<Semester>, ISemesterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SemesterService(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CurrentSemesterRegistrationIsOpenedAsync()
        {
            var CurrentSemester = await GetCurrentSemesterAsync();
            var Now = DateTime.Now;
            if (CurrentSemester is not null)
                return (CurrentSemester.CourseRegistrationStartDate <= Now && CurrentSemester.CourseRegistrationEndDate >= Now);
            else
                return false;
        }

        public async Task<Semester> GetCurrentSemesterAsync()
        {
            var Today = DateTime.Today;
            return await _unitOfWork.Repository<Semester>().GetAsQueryAble().Where(s => s.StartDate <= Today && s.EndDate >= Today).Include(s => s.AcademicYear).FirstOrDefaultAsync();
        }

        public async Task<List<Semester>> GetSemestersByAcademicYearAsync(int academicYearId)
        {
            return await _unitOfWork.Repository<Semester>().GetAsQueryAble().Where(s => s.AcademicYearId==academicYearId).Include(s => s.AcademicYear).ToListAsync();
        }

    }
}
