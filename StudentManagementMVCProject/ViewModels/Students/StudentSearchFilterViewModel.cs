using StudentManagementMVCProject.Enums;

namespace StudentManagementMVCProject.ViewModels.Students
{
    public class StudentSearchFilterViewModel
    {
        public string? StudentNumber { get; set; }
        public string? FullName { get; set; }
        public string? NationalId { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public AcademicLevel? AcademicLevel { get; set; }
        public StudentStatus? StudentStatus { get; set; }

    }
}
