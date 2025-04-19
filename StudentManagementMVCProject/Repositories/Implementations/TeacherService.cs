using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class TeacherService : GenericRepository<Teacher>, ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public TeacherService(ApplicationDbContext context, IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper) : base(context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Teacher> AddTeacherUserAsync(AddTeacherDTO model)
        {
            var isExistBefore = await _userManager.FindByEmailAsync(model.Email);
            if (isExistBefore != null)
            {
                return null;
            }
            var newUser = _mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Teacher");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return null;
            }
            var newTeacher = _mapper.Map<Teacher>(model);
            newTeacher.UserId = newUser.Id;
            await _unitOfWork.Repository<Teacher>().AddAsync(newTeacher);
            await _unitOfWork.CompleteAsync();
            return newTeacher;
        }

        public async Task<IEnumerable<TeacherNameViewModel>> GetTeachersNameAsync()
        {
            return await _unitOfWork.Repository<Teacher>().GetAsQueryAble()
                .Include(s => s.User)
                .ProjectTo<TeacherNameViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
