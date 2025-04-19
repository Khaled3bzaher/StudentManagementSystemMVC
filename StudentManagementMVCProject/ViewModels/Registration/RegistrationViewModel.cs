using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.ViewModels.Courses;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Registration
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "يجب تحديد الفصل الدراسي")]
        public Semester Semester { get; set; } = new Semester(); // تهيئة افتراضية

        [Required(ErrorMessage = "يجب توفير قائمة المقررات المتاحة")]
        public List<CourseViewModel> AvailableCourses { get; set; } = new List<CourseViewModel>(); // تهيئة كقائمة فارغة

        [Range(0, int.MaxValue, ErrorMessage = "الحد الأقصى للساعات يجب أن يكون قيمة صحيحة")]
        public int AllowedCreditHours { get; set; }

        [Required(ErrorMessage = "يجب اختيار مقرر واحد على الأقل")]
        public List<int> SelectedCourseIds { get; set; } = new List<int>();
    }
}
