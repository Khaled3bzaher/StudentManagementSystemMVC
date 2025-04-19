using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Courses
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        [Display(Name = "Credit Hour")]
        public int CreditHour { get; set; }
        [Display(Name = "Is Shared Between Departments")]

        public bool isCommon { get; set; }
        [Display(Name = "Active")]
        public bool isActive { get; set; }
        public string Department { get; set; }
        [Display(Name = "Prerequisite Course")]

        public string PrerequisiteCourse { get; set; }
    }
}
