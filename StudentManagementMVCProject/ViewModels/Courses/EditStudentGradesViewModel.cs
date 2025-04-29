using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.ViewModels.Courses
{
    public class EditStudentGradesViewModel
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int SemesterId { get; set; }
        [Range(0,20,ErrorMessage ="Midterm Must be 0 to 20")]
        [Remote(action: "ValidateGrade", controller: "Validations", AdditionalFields = nameof(Final) + "," + nameof(Assignments) + "," + nameof(Projects) + "," + nameof(AttendanceGrade))]
        public float? MidTerm { get; set; }
        [Range(0, 50, ErrorMessage = "Final Must be 0 to 50")]

        [Remote(action: "ValidateGrade", controller: "Validations", AdditionalFields = nameof(MidTerm) + "," + nameof(Assignments) + "," + nameof(Projects) + "," + nameof(AttendanceGrade))]
        public float? Final { get; set; }
        [Range(0, 20, ErrorMessage = "Assignments Must be 0 to 20")]
        [Remote(action: "ValidateGrade", controller: "Validations", AdditionalFields = nameof(MidTerm) + "," + nameof(Final) + "," + nameof(Projects) + "," + nameof(AttendanceGrade))]
        public float? Assignments { get; set; }
        [Range(0, 20, ErrorMessage = "Projects Must be 0 to 20")]

        [Remote(action: "ValidateGrade", controller: "Validations", AdditionalFields = nameof(MidTerm) + "," + nameof(Final) + "," + nameof(Assignments) + "," + nameof(AttendanceGrade))]
        public float? Projects { get; set; }
        [Range(0, 20, ErrorMessage = "Attendance Must be 0 to 20")]

        [Remote(action: "ValidateGrade", controller: "Validations", AdditionalFields = nameof(MidTerm) + "," + nameof(Final) + "," + nameof(Assignments) + "," + nameof(Projects))]
        public float? AttendanceGrade { get; set; }
        public float? TotalGrade { get; set; }
        public string? LetterGrade { get; set; }
        public bool CanEdit { get; set; } = true;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
