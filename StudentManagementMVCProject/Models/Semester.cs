using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementMVCProject.Models
{
    public class Semester
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Max Length for Semester is 50")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Semester Start Date")]

        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Semester End Date")]

        public DateTime EndDate { get; set; }
        [Required]
        [Display(Name = "Registration Start Date")]

        public DateTime CourseRegistrationStartDate { get; set; }
        [Required]
        [Display(Name = "Registration End Date")]

        public DateTime CourseRegistrationEndDate { get; set; }
        [ForeignKey(nameof(AcademicYear))]
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
    }
}
