using StudentManagementMVCProject.Enums;

namespace StudentManagementMVCProject.ViewModels.Courses
{
    public class CourseStudentsListViewModel
    {
        public int StudentId { get; set; }
        public string StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CourseStatus { get; set; }
        public AcademicLevel AcademicLevel { get; set; }
        public float? MidTerm { get; set; }
        public float? Final { get; set; }
        public float? Assignments { get; set; }
        public float? Projects { get; set; }
        public float? AttendanceGrade { get; set; }
        public float? TotalGrade { get; set; }

        
    }
}
