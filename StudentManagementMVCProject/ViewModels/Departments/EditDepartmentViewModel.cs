using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Departments
{
    public class EditDepartmentViewModel
    {
        public int Id { get; set; }
        [Remote(action: "VerifyDepartmentCode", controller: "Validations",AdditionalFields =nameof(Id))]
        [Display(Name = "Department Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Enter Department Name Please .!")]
        [Remote(action: "VerifyName", controller: "Validations")]
        [Display(Name = "Department Name")]
        public string Name { get; set; }


        [Display(Name = "Department Active")]
        public bool isActive { get; set; }

        [Display(Name = "Department Head")]

        public int? HeadTeacherID { get; set; }
    }
}
