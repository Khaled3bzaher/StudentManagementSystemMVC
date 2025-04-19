using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models

{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Range(1,4)]
        public int CreditHour { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public bool isCommon { get; set; }

        public bool isActive { get; set; }

        [ForeignKey(nameof(PrerequisiteCourse))]
        public int? PrerequisiteCourseId { get; set; }
        public Course? PrerequisiteCourse { get; set; }


        public ICollection<CourseSchedule> CourseSchedules  { get; set; }= new HashSet<CourseSchedule>();

    }
}
