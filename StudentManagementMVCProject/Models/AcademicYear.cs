using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.Models
{
    public class AcademicYear
    {
        public int Id { get; set; }
        [Remote(action: "VerifyAcademicYearName", controller: "Validations",AdditionalFields =nameof(Id))]
        public string Name { get; set; }

        public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
    }
}
