using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.DTOs.Teachers
{
    public class AddTeacherDTO
    {
        [Required(ErrorMessage = "Enter First Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Firs tName is 50 Letters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Last Name is 50 Letters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter NationalId Please .!")]
        [MaxLength(14, ErrorMessage = "Max Length For NationalId is 14 Letters")]
        public string NationalId { get; set; }
        [Required(ErrorMessage = "Enter Phone Please .!")]
        [Phone(ErrorMessage = "Enter Valid Phone .!")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Enter Department Please .!")]
        public int DepartmentId { get; set; }
        public SelectList? Departments { get; set; }
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Enter Qualification Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Qualification is 50 Letters")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Enter Email Please .!")]
        [EmailAddress(ErrorMessage = "Enter Valid Email .!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password Please .!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
