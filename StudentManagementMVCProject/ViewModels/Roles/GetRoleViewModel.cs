using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Roles

{
    public class GetRoleViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Must Enter Role Name")]
        public string Name { get; set; }
        public int UserCount { get; set; }
    }
}
