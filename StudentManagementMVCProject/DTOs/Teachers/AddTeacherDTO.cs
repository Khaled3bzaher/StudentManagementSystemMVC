using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.DTOs.Teachers
{
    public class AddTeacherDTO
    {
        [Required(ErrorMessage = "Enter First Name Please .!")]
        [Remote(action: "VerifyName", controller: "Validations")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name Please .!")]
        [Display(Name = "Last Name")]
        [Remote(action: "VerifyName", controller: "Validations")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter NationalId Please .!")]
        [Display(Name = "National ID")]
        [Remote(action: "VerifyNationalId", controller: "Validations")]
        public string NationalId { get; set; }

        [Display(Name = "Phone Number")]
        [Remote(action: "VerifyNumber", controller: "Validations")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [Remote(action: "VerifyBirthDay", controller: "Validations")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Enter Department Please .!")]
        public int DepartmentId { get; set; }
        public SelectList? Departments { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter Hire Date Please .!")]
        [Remote(action: "VerifyHireDate", controller: "Validations",AdditionalFields = "DateOfBirth")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Enter Qualification Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Qualification is 50 Letters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Qualification must contain letters only.")]
        public string Qualification { get; set; }

        [Remote(action: "VerifyEmail", controller: "Validations")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password Please .!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
