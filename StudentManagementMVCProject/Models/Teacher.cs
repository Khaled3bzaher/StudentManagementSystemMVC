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
    public class Teacher
    {
        public int Id { get; set; }
        
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User? User { get; set; }
       
        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        [Required]
        public string Qualification { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        public ICollection<CourseSchedule> CourseSchedules { get; set; } = new List<CourseSchedule>();
    }
}
