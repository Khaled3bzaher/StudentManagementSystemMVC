using StudentManagementMVCProject.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Students
{
    public class StudentDetailsViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string StudentNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ParentNumber { get; set; }
        public string Department { get; set; }
        public StudentStatus Status { get; set; }
        public AcademicLevel Level { get; set; }
        public DateTime EnrollmentDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string ProfilePictureURL { get; set; }
        public bool isActive { get; set; }


    }
}
