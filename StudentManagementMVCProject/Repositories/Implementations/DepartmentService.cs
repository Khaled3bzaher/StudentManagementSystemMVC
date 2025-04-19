using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Departments;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class DepartmentService : GenericRepository<Department>, IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(ApplicationDbContext context,IUnitOfWork unitOfWork , IMapper mapper) : base(context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DepartmentViewModel> GetDepartmentByIdAsync(int? id)
        {
            return await _unitOfWork.Repository<Department>().GetAsQueryAble().Where(d=>d.Id==id).Include(d => d.HeadTeacher).ThenInclude(t => t.User).ProjectTo<DepartmentViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

        }

        public async Task<EditDepartmentViewModel> GetDepartmentByIdForEditAsync(int? id)
        {
            return await _unitOfWork.Repository<Department>().GetAsQueryAble().Where(d => d.Id == id).Include(d => d.HeadTeacher).ThenInclude(t => t.User).ProjectTo<EditDepartmentViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<DepartmentNameCodeViewModel>> GetDepartmentsNamesCodesAsync()
        {
            return await _unitOfWork.Repository<Department>().GetAsQueryAble().ProjectTo<DepartmentNameCodeViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<IEnumerable<TeacherNameViewModel>> GetDepartmentTeachersByIdAsync(int? id)
        {
            return await _unitOfWork.Repository<Teacher>().GetAsQueryAble().Where(t=>t.DepartmentId==id).ProjectTo<TeacherNameViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<IEnumerable<DepartmentViewModel>> GetDepartmentWithHeadsAsync()
        {
            return await _unitOfWork.Repository<Department>().GetAsQueryAble().Include(d=>d.HeadTeacher).ThenInclude(t=>t.User).ProjectTo<DepartmentViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
