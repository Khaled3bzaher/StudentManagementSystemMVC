using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models
{
    /// <summary>
    /// Semester Record For Each Student
    /// </summary>
    public class AcademicRecord
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [ForeignKey(nameof(Semester))]
        public int SemesterId { get; set; }
        public Semester? Semester { get; set; }
        public float SemesterGPA { get; set; }
        public float CumulativeGPA { get; set; }
        public int TotalCredits { get; set; }
        public int TotalCourses { get; set; }
        public string AcademicStanding { get; set; }
    }
}
