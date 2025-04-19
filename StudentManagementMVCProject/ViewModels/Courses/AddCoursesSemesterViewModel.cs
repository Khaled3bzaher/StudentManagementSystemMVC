using StudentManagementMVCProject.Models;

namespace StudentManagementMVCProject.ViewModels.Courses
{
    public class AddCoursesSemesterViewModel
    {
        public int SemesterId { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();

        public List<CourseTeacher> CourseTeacherPairs { get; set; } = new List<CourseTeacher>();
    }
    public class CourseTeacher
    {
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public int? ScheduleId { get; set; }
    }
}
