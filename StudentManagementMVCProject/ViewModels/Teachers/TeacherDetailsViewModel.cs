using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Teachers
{
    public class TeacherDetailsViewModel
    {
        public int Id { get; set; }
        [Display(Name ="Full Name")]
        public string FullName { get; set; }
        [Display(Name = "National Id")]
        public string NationalId { get; set; }
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string ProfilePictureURL { get; set; }
        public string Qualification { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime DateOfBirth { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }
        [Display(Name = "Is Active?")]
        public bool isActive { get; set; }
    }
}
