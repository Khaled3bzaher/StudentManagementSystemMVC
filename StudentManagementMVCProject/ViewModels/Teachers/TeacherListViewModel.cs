using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Teachers
{
    public class TeacherListViewModel
    {
        public int Id { get; set; }
        [Display(Name ="Teacher Name")]
        public string FullName { get; set; }
        [Display(Name = "National ID")]
        public string NationalId { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
    }
}
