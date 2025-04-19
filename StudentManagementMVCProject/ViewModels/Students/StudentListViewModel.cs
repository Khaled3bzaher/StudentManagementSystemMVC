using StudentManagementMVCProject.Enums;

namespace StudentManagementMVCProject.ViewModels.Students
{
    /// <summary>
    /// TO VIEW IN TABLES
    /// ------------------------------------------
    /// First Name, Last Name
    /// StudentNumber
    /// Email
    /// National Id
    /// Phone Number
    /// Department
    /// Academic Level
    /// Status
    /// </summary>
    public class StudentListViewModel
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public AcademicLevel AcademicLevel { get; set; }
        public StudentStatus StudentStatus { get; set; }
    }
}
