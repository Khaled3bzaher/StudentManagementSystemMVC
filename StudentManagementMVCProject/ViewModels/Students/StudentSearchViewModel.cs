namespace StudentManagementMVCProject.ViewModels.Students
{
    public class StudentSearchViewModel
    {
        public IEnumerable<StudentListViewModel> Students { get; set; }
        public StudentSearchFilterViewModel Filter { get; set; }
    }
}
