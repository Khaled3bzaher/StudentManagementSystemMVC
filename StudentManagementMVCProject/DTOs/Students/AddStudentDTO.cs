using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Validations;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.DTOs.Students
{
    public class AddStudentDTO
    {
        [Display(Name ="Student Number")]
        [Remote(action: "VerifyStudentNumber", controller: "Validations")]
        public string? StudentNumber { get; set; }

        [Required(ErrorMessage ="Enter First Name Please .!")]
        [MaxLength(50,ErrorMessage ="Max Length For Firs tName is 50 Letters")]
        [Display(Name = "First Name")]
        [Remote(action: "VerifyName", controller: "Validations")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Last Name is 50 Letters")]
        [Display(Name = "Last Name")]
        [Remote(action: "VerifyName", controller: "Validations")]
        public string LastName { get; set; }

        [Remote(action: "VerifyNationalId", controller: "Validations")]
        [Display(Name = "National ID")]

        public string NationalId { get; set; }


        [Display(Name = "Phone Number")]
        [Remote(action: "VerifyNumber", controller: "Validations")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Parent Number")]
        [Remote(action: "VerifyParentNumber", controller: "Validations")]
        public string? ParentPhone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [Remote(action: "VerifyBirthDay", controller: "Validations")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100, ErrorMessage = "Max Length For Address is 100 Letters")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Choose Department Please .!")]
        [Display(Name = "Department")]

        public int DepartmentId { get; set; }

        [Remote(action: "VerifyEmail", controller: "Validations")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password Please .!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Choose Level of Student Please .!")]
        public AcademicLevel Level { get; set; }

        [Required(ErrorMessage = "Choose Status Please .!")]
        public StudentStatus Status { get; set; }


        public SelectList? Departments { get; set; }
        public SelectList? StudentStatus { get; set; }
        public SelectList? AcademicLevel { get; set; }

    }
}
