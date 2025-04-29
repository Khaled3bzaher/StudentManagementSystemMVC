namespace StudentManagementMVCProject.ViewModels.Teachers
{
    public class TeacherCoursesViewModel
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public List<TeacherCourseEnrollmentsViewModel> TeacherCourseEnrollments { get; set; }

    }
}
