namespace StudentManagementMVCProject.ViewModels.Students
{
    public class StudentCoursesViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<StudentCourseEnrollment> CoursesEnrollment { get; set; }
    }

    public class StudentCourseEnrollment
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }

        public float MidTerm { get; set; }
        public float Final { get; set; }
        public float Assignments { get; set; }
        public float Projects { get; set; }
        public float AttendanceGrade { get; set; }
        public float TotalGrade { get; set; }
        public string LetterGrade { get; set; }
    }
}
