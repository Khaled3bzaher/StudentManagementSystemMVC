using StudentManagementMVCProject.DTOs.Roles;

namespace StudentManagementMVCProject.Repositories.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleWithUserCount>> GetRolesWithUserCountAsync();
    }
}