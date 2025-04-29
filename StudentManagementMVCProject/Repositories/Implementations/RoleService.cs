using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.DTOs.Roles;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Roles;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleWithUserCount>> GetRolesWithUserCountAsync()
        {
            return await _context.Set<RoleWithUserCount>()
                .ToListAsync();
        }
    }
}
