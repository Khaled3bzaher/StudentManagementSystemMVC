using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Departments
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        [Display(Name="Head")]
        public string DepartmentHeadName { get; set; }
        [Display(Name = "Is Avaliable?")]
        public bool isActive { get; set; }
        [Display(Name = "Students Count")]
        public int StudentsCount { get; set; }
        [Display(Name = "Teachers Count")]
        public int TeachersCount { get; set; }
        [Display(Name = "Courses Count")]
        public int CoursesCount { get; set; }

    }
}
