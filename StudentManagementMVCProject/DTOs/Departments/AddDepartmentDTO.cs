using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.DTOs.Departments
{
    public class AddDepartmentDTO
    {
        [Remote(action: "VerifyDepartmentCode", controller: "Validations")]
        [Display(Name = "Department Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Enter Department Name Please .!")]
        [Remote(action: "VerifyName", controller: "Validations")]
        [Display(Name ="Department Name")]
        public string Name { get; set; }


        [Display(Name ="Department Active")]
        public bool isActive { get; set; }

        

    }
}
