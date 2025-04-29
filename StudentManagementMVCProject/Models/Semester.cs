using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementMVCProject.Models
{
    public class Semester
    {
        public int Id { get; set; }
        [Remote("VerifySemesterName", "Validations", AdditionalFields = nameof(Id) + "," + nameof(AcademicYearId))]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Semester Start Date")]
        [Required]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Semester End Date")]
        [Remote("VerifySemesterDates", "Validations", AdditionalFields = nameof(StartDate) + "," + nameof(Id))]
        public DateTime EndDate { get; set; }
        [Required]
        [Display(Name = "Registration Start Date")]

        public DateTime CourseRegistrationStartDate { get; set; }
        [Display(Name = "Registration End Date")]
        [Remote("VerifyRegistrationDates", "Validations", AdditionalFields = nameof(StartDate) + "," + nameof(EndDate) + "," + nameof(CourseRegistrationStartDate) + "," + nameof(Id))]
        public DateTime CourseRegistrationEndDate { get; set; }
        [ForeignKey(nameof(AcademicYear))]
        [Display(Name = "Academic Year")]

        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
    }
}
