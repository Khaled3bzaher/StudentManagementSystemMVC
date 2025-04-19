using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Roles
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessage ="Must Enter Role Name")]
        public string Name { get; set; }
    }
}
