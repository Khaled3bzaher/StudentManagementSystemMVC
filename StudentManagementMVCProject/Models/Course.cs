using Microsoft.AspNetCore.Mvc;
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
        [Remote(action: "VerifyCourseCode", controller: "Validations",AdditionalFields =nameof(Id))]
        [Display(Name ="Course Code")]
        public string Code { get; set; }

        [Required(ErrorMessage ="Course Name is Required ..!")]
        [Remote(action: "VerifyName", controller: "Validations")]
        [Display(Name = "Course Name")]
        public string Name { get; set; }
        [Range(1,4,ErrorMessage ="Credit Hours Must be 1 to 4 ..!")]
        [Display(Name = "Course Hours")]

        public int CreditHour { get; set; }

        [ForeignKey(nameof(Department))]
        [Display(Name = "Course Department")]

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        [Display(Name = "Is Shared Course?")]

        public bool isCommon { get; set; }
        [Display(Name = "Is Avaliable Course?")]

        public bool isActive { get; set; }

        [ForeignKey(nameof(PrerequisiteCourse))]
        [Display(Name = "Course Prerequisite")]

        public int? PrerequisiteCourseId { get; set; }
        public Course? PrerequisiteCourse { get; set; }


        public ICollection<CourseSchedule> CourseSchedules  { get; set; }= new HashSet<CourseSchedule>();

    }
}
