using StudentManagementMVCProject.ViewModels.Shared;

namespace StudentManagementMVCProject.ViewModels.Students
{
    public class StudentSearchViewModel
    {
        public PagedResult<StudentListViewModel> Students { get; set; } = new PagedResult<StudentListViewModel>();
        public StudentSearchFilterViewModel Filter { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 4;
    }
}
