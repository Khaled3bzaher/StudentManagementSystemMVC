using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models
{
    public class Grade
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Enrollment))]
        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }
        public float? MidTerm { get; set; } = 0;
        public float? Final { get; set; } = 0;
        public float? Assignments { get; set; } = 0;
        public float? Projects { get; set; } = 0;
        public float? AttendanceGrade { get; set; } = 0;
        public float? TotalGrade { get; set; } = 0;
        public string? LetterGrade { get; set; }
        public string? Comment { get; set; }
        public DateTime LastUpdated { get; set; }
     
    }
}
