using Microsoft.AspNetCore.Identity;
using StudentManagementMVCProject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models
{
    public class Student
    {
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User? User { get; set; }
        [Required]
        [MaxLength(14)]
        public string? StudentNumber { get; set; }
        [MaxLength(100)]
        public string? Address { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? ParentPhone { get; set; }

        [ForeignKey(nameof(Department))]
        [Required]

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        [Required]
        [MaxLength(10)]
        public StudentStatus Status { get; set; }
        [Required]
        public DateTime EnrollmentDate { get; set; }= DateTime.Now;
        [Required]
        public AcademicLevel AcademicLevel { get; set; }
       
        public ICollection<AcademicRecord> AcademicRecords { get; set; }= new List<AcademicRecord>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

}
