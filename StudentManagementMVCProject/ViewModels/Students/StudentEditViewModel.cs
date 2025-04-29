using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementMVCProject.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Students
{
    public class StudentEditViewModel
    {
        public int Id { get; set; }


        [Remote(action: "VerifyStudentNumber", controller: "Validations", AdditionalFields = "Id")]
        public string? StudentNumber { get; set; }

        [Required(ErrorMessage = "Enter First Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For First Name is 50 Letters")]
        [Remote(action: "VerifyName", controller: "Validations")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Last Name is 50 Letters")]
        [Remote(action: "VerifyName", controller: "Validations")]
        public string LastName { get; set; }

        [Remote(action: "VerifyNationalId", controller: "Validations",AdditionalFields = "Email")]
        public string NationalId { get; set; }

        [Remote(action: "VerifyNumber", controller: "Validations", AdditionalFields = "Email")]
        public string PhoneNumber { get; set; }

        [Remote(action: "VerifyParentNumber", controller: "Validations")]
        public string? ParentPhone { get; set; }

        [DataType(DataType.Date)]
        [Remote(action: "VerifyBirthDay", controller: "Validations")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100, ErrorMessage = "Max Length For Address is 100 Letters")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Choose Department Please .!")]
        public int DepartmentId { get; set; }


        [EmailAddress(ErrorMessage = "Enter a Valid Email .!")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and Confirmation do not match.")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Choose Level of Student Please .!")]
        public AcademicLevel AcademicLevel { get; set; }

        [Required(ErrorMessage = "Choose Status Please .!")]
        public StudentStatus Status { get; set; }

        public string? ProfilePictureURL { get; set; }

        public IFormFile? NewProfilePicture { get; set; }

        public bool IsActive { get; set; }


    }
}
