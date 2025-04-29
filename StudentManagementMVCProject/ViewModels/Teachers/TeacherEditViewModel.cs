using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Teachers
{
    public class TeacherEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter First Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For First Name is 50 Letters")]
        [Remote(action: "VerifyName", controller: "Validations")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Last Name is 50 Letters")]
        [Remote(action: "VerifyName", controller: "Validations")]
        public string LastName { get; set; }
        [Remote(action: "VerifyNationalId", controller: "Validations", AdditionalFields = "Email")]
        public string NationalId { get; set; }

        [Remote(action: "VerifyNumber", controller: "Validations", AdditionalFields = "Email")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        [Remote(action: "VerifyBirthDay", controller: "Validations")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter Hire Date Please .!")]
        [Remote(action: "VerifyHireDate", controller: "Validations", AdditionalFields = "DateOfBirth")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Choose Department Please .!")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Enter Qualification Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Qualification is 50 Letters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Qualification must contain letters only.")]
        public string Qualification { get; set; }

        [EmailAddress(ErrorMessage = "Enter a Valid Email .!")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and Confirmation do not match.")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmPassword { get; set; }
        public string? ProfilePictureURL { get; set; }

        public IFormFile? NewProfilePicture { get; set; }

        public bool IsActive { get; set; }
    }
}
