using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

   

        [ForeignKey(nameof(CourseSchedule))]
        public int CourseScheduleId { get; set; }
        public CourseSchedule? CourseSchedule { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        [Required]
        [MaxLength(50)]
        public string GradeStatus { get; set; }

        public Grade Grade { get; set; }
    }
}
