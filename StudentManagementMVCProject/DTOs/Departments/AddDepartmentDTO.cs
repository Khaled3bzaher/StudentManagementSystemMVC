using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.DTOs.Departments
{
    public class AddDepartmentDTO
    {
        [Required(ErrorMessage = "Enter Department Code Please .!")]
        [MaxLength(10, ErrorMessage = "Max Length For Department Code is 10 Letters")]
        [Display(Name = "Department Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Enter Department Name Please .!")]
        [MaxLength(50, ErrorMessage = "Max Length For Department Name is 50 Letters")]
        [Display(Name ="Department Name")]
        public string Name { get; set; }


        [Display(Name ="Department Active")]
        public bool isActive { get; set; }

        [Display(Name ="Department Head")]

        public int? HeadTeacherID { get; set; }

    }
}
