using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.Models
{
    public class AcademicYear
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(15,ErrorMessage ="Max Length for Academic Year is 15")]
        public string Name { get; set; }

        public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
    }
}
