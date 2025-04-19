using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models
{
    public class CourseSchedule
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        [ForeignKey(nameof(Semester))]
        public int SemesterId { get; set; }
        public Semester? Semester { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
