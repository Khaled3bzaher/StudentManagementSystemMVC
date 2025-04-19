using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models
{
    public class Department
    {

        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Code { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [ForeignKey(nameof(HeadTeacher))]
        public int? HeadTeacherID { get; set; }
        public Teacher? HeadTeacher { get; set; }
        public bool isActive { get; set; }=true;
        
        //HAS
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        //OFFERS
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
