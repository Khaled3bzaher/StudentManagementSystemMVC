using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using System.Text.RegularExpressions;
using System.Web.Helpers;

namespace StudentManagementMVCProject.Validations
{
    public class ValidationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return NotFound();
        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyNationalId(string nationalId, string email = null)
        {

            if (string.IsNullOrEmpty(nationalId))
            {
                return Json("National Id is Required .. !");
            }

            if (nationalId.Length != 14)
            {
                return Json("National Id Length Must be 14 Digits ..!");
            }

            if (!nationalId.All(char.IsDigit))
            {
                return Json("National Id Must be Numbers Only ..!");
            }

            if (!IsValidEgyptianNationalId(nationalId))
            {
                return Json("Egyption National Id is Not Valid ..!");
            }

            var existingPerson = await _unitOfWork.Repository<User>().GetAsQueryAble().Select(u => new User
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                NationalId = u.NationalId,
                Email = u.Email
            }).FirstOrDefaultAsync(u => u.NationalId == nationalId && u.Email != email);

            if (existingPerson != null)
            {
                return Json($"National Id Exists Before By {existingPerson.FirstName} {existingPerson.LastName} ..!");
            }

            return Json(true);
        }
        private bool IsValidEgyptianNationalId(string nationalId)
        {
            if (nationalId.Length != 14) return false;

            int year = int.Parse(nationalId.Substring(1, 2));
            int month = int.Parse(nationalId.Substring(3, 2));
            int day = int.Parse(nationalId.Substring(5, 2));

            if (month < 1 || month > 12) return false;
            if (day < 1 || day > 31) return false;

            char centuryDigit = nationalId[0];
            if (centuryDigit != '2' && centuryDigit != '3') return false;


            return true;
        }

        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyNumber(string phoneNumber, string email = null)
        {

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Json("Phone Number is Required .. !");
            }


            if (!phoneNumber.All(char.IsDigit))
            {
                return Json("Phone Number Must be Numbers Only ..!");
            }

            if (!phoneNumber.StartsWith("01"))
            {
                return Json("Phone Number Must Starts with 01 ..!");
            }


            if (phoneNumber.Length != 11)
            {
                return Json("Phone Number Length Must be 11 Digits ..!");
            }

            var secondDigit = phoneNumber[2];
            var validSecondDigits = new[] { '0', '1', '2', '5' };
            if (!validSecondDigits.Contains(secondDigit))
            {
                return Json("Phone Number Must Follow Egyptian Communcation Providers ..!");

            }

            var existingPerson = await _unitOfWork.Repository<User>().GetAsQueryAble().Select(u => new User
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email
            }).FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.Email != email);

            if (existingPerson != null)
            {
                return Json($"Phone Number is Used Before By {existingPerson.FirstName} {existingPerson.LastName} ..!");
            }

            return Json(true);

        }

        



        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyParentNumber(string parentPhone)
        {




            if (!parentPhone.All(char.IsDigit))
            {
                return Json("Parent Number Must be Numbers Only ..!");
            }

            if (!parentPhone.StartsWith("01"))
            {
                return Json("Parent Number Must Starts with 01 ..!");
            }


            if (parentPhone.Length != 11)
            {
                return Json("Parent Number Length Must be 11 Digits ..!");
            }

            var secondDigit = parentPhone[2];
            var validSecondDigits = new[] { '0', '1', '2', '5' };
            if (!validSecondDigits.Contains(secondDigit))
            {
                return Json("Parent Number Must Follow Egyptian Communcation Providers ..!");

            }

            return Json(true);

        }

        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyStudentNumber(string studentNumber, int Id)
        {

            if (!studentNumber.All(char.IsDigit))
            {
                return Json("Student Number Must be Numbers Only ..!");
            }
            if (!(studentNumber.Length >= 11 && studentNumber.Length <= 14))
            {
                return Json("Student Number Length Must be 11 to 14 Digits ..!");
            }
            var existingPerson = await _unitOfWork.Repository<Student>().GetAsQueryAble().Include(s => s.User).Select(s => new
            {
                StudentNumber = s.StudentNumber,
                Id = s.Id,
                User = new User
                {
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName
                }
            }).FirstOrDefaultAsync(s => s.StudentNumber == studentNumber && s.Id != Id);

            if (existingPerson != null)
            {
                return Json($"National Id Exists Before By {existingPerson.User.FirstName} {existingPerson.User.LastName} ..!");
            }

            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyEmail(string email)
        {

            if (string.IsNullOrEmpty(email))
            {
                return Json("Email is Required .. !");
            }
            var emailRegex = new Regex(@"^[a-zA-Z0-9_]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            if (!emailRegex.IsMatch(email))
            {
                return Json("Email Must Contains English Letters , @ and . xx ..!");
            }
            var existingPerson = await _unitOfWork.Repository<User>().GetAsQueryAble().Select(s => new User
            {
                Email = s.Email,
                FirstName = s.FirstName,
                LastName = s.LastName
            }).FirstOrDefaultAsync(s => s.Email == email);
            if (existingPerson != null)
            {
                return Json($"Email Is Used Before By {existingPerson.FirstName} {existingPerson.LastName} ..!");
            }
            return Json(true);

        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyBirthDay(DateTime DateOfBirth)
        {

            if (DateOfBirth > DateTime.Now)
            {
                return Json("Birthday Must be in The Past ..!");
            }

            var age = DateTime.Now.Year - DateOfBirth.Year;
            if (DateOfBirth > DateTime.Now.AddYears(-age)) age--;

            if (age <= 18)
            {
                return Json("Age Must be Above 18 Years ..!");
            }

            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyName(string firstName = null, string lastName = null, string Name = null)
        {

            string name = null;
            if (!string.IsNullOrEmpty(firstName))
            {
                name = firstName;
            }
            else if (!string.IsNullOrEmpty(lastName))
            {
                name = lastName;
            }
            else if (!string.IsNullOrEmpty(Name))
            {
                name = Name;
            }
            else
            {
                return Json(false);
            }
            var regex = new Regex(@"^[a-zA-Z\s]+$");
            if (!regex.IsMatch(name))
            {
                return Json("Name Must be Characters Only ..!");
            }
            if (!(name.Length >= 4 && name.Length <= 50))
            {
                return Json("Name Must be 4 to 50 characters ..!");
            }
            return Json(true);

        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyHireDate(DateTime HireDate, DateTime? dateOfBirth)
        {


            if (HireDate > DateTime.Now)
            {
                return Json("Hire Date Must be in The Past ..!");
            }
            if (dateOfBirth.HasValue)
            {
                DateTime minHireDate = dateOfBirth.Value.AddYears(18);

                if (HireDate < minHireDate)
                {
                    return Json("Hire Date must be at least 18 years after Date of Birth.");
                }
            }
            return Json(true);
        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> ValidateGrade(
            float? midTerm, float? final, float? assignments,
            float? projects, float? attendanceGrade)
        {



            var sum = (midTerm ?? 0) + (final ?? 0) + (assignments ?? 0) +
                      (projects ?? 0) + (attendanceGrade ?? 0);

            if (sum > 100)
            {
                return Json("مجموع الدرجات يجب أن يكون 100 أو أقل");
            }

            return Json(true);
        }

        //Course Validations
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyCourseCode(string Code, int Id)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return Json("Course Code is required.");
            }

            if (!(Code.Length >= 3 && Code.Length <= 10))
            {
                return Json("Course Code must be between 3 and 10 characters.");
            }

            var regex = new Regex(@"^[a-zA-Z0-9]+$");
            if (!regex.IsMatch(Code))
            {
                return Json("Course Code must contain only English letters and numbers.");
            }

            var existingCourse = await _unitOfWork.Repository<Course>().GetAsQueryAble().Select(c => new Course
            {
                Code = c.Code,
                Name = c.Name,
                Id = c.Id
            }).FirstOrDefaultAsync(c => c.Code == Code && c.Id != Id);
            if (existingCourse != null)
            {
                return Json($"Cours Code Is Used Before By {existingCourse.Code} {existingCourse.Name} ..!");
            }

            return Json(true);
        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyDepartmentCode(string Code, int Id)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return Json("Department Code is required.");
            }

            if (!(Code.Length >= 2 && Code.Length <= 10))
            {
                return Json("Department Code must be between 2 and 10 characters.");
            }

            var regex = new Regex(@"^[a-zA-Z]+$");
            if (!regex.IsMatch(Code))
            {
                return Json("Department Code must contain only English letters and numbers.");
            }

            var existingDepartment = await _unitOfWork.Repository<Department>().GetAsQueryAble().Select(d => new Department
            {
                Code = d.Code,
                Name = d.Name,
                Id = d.Id
            }).FirstOrDefaultAsync(d => d.Code == Code && d.Id != Id);
            if (existingDepartment != null)
            {
                return Json($"Department Code Is Used Before By {existingDepartment.Code} {existingDepartment.Name} ..!");
            }

            return Json(true);
        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyAcademicYearName(string Name, int Id)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return Json("Academic Year Name is required.");
            }

            var regex = new Regex(@"^\d{4}[-/]\d{4}$");
            if (!regex.IsMatch(Name))
            {
                return Json("Academic Year Name must be Like 2025-2026 or 2025/2026");
            }
            string[] parts = Name.Split(new char[] { '-', '/' });
            int firstYear = int.Parse(parts[0]);
            int secondYear = int.Parse(parts[1]);
            if (secondYear != firstYear + 1)
                return Json("Academic Year Duration must be one Year 202X TO 202(X+1)");

            var existingYear = await _unitOfWork.Repository<AcademicYear>().GetAsQueryAble().AnyAsync(a => a.Name == Name && a.Id != Id);
            if (existingYear)
            {
                return Json($"Academic Year Name Is Used Before ..!");
            }

            return Json(true);
        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifySemesterDates(DateTime startDate, DateTime endDate, int id)
        {
            if (startDate == default(DateTime))
            {
                return Json("Start Date is required.");
            }

            if (endDate == default(DateTime))
            {
                return Json("End Date is required.");
            }

            if (endDate <= startDate)
            {
                return Json("End Date must be greater than Start Date.");
            }

            var overlappingSemester = await _unitOfWork.Repository<Semester>()
                .GetAsQueryAble()
                .Where(s => s.Id != id)
                .FirstOrDefaultAsync(s =>
                    (startDate >= s.StartDate && startDate <= s.EndDate) ||
                    (endDate >= s.StartDate && endDate <= s.EndDate) ||
                    (startDate <= s.StartDate && endDate >= s.EndDate));

            if (overlappingSemester != null)
            {
                return Json($"This semester overlaps with another semester: {overlappingSemester.Name} ({overlappingSemester.StartDate.ToString("dd/MM/yyyy")} - {overlappingSemester.EndDate.ToString("dd/MM/yyyy")}).");
            }

            return Json(true);


        }

        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifyRegistrationDates(DateTime courseRegistrationStartDate, DateTime courseRegistrationEndDate, DateTime startDate, DateTime endDate, int id)
        {
            if (courseRegistrationStartDate == default(DateTime))
            {
                return Json("Registration Start Date is required.");
            }

            if (courseRegistrationEndDate == default(DateTime))
            {
                return Json("Registration End Date is required.");
            }

            if (startDate == default(DateTime))
            {
                return Json("Semester Start Date is required.");
            }

            if (endDate == default(DateTime))
            {
                return Json("Semester End Date is required.");
            }

            if (courseRegistrationStartDate < startDate)
            {
                return Json("Registration Start Date must be on or after Semester Start Date.");
            }

            if (courseRegistrationEndDate < startDate || courseRegistrationEndDate > endDate)
            {
                return Json("Registration End Date must be between Semester Start Date and End Date.");
            }

            if (courseRegistrationEndDate <= courseRegistrationStartDate)
            {
                return Json("Registration End Date must be after Registration Start Date.");
            }

           

            return Json(true);
        }
        [AcceptVerbs("GET", "POST")]
        [AjaxOnly]
        public async Task<IActionResult> VerifySemesterName(string Name, int Id,int AcademicYearId)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return Json("Semester Name is required.");
            }

            if (!(Name.Length >= 4 && Name.Length <= 50))
            {
                return Json("Semester Name must be between 4 and 50 characters.");
            }

            var regex = new Regex(@"^(?=.*[a-zA-Z])[a-zA-Z0-9\s]+$");
            if (!regex.IsMatch(Name))
            {
                return Json("Semester Name must contain only English letters and numbers.");
            }

            var existingSemester = await _unitOfWork.Repository<Semester>().GetAsQueryAble().Include(s=>s.AcademicYear)
                .Where(s => s.Name == Name && s.Id != Id && s.AcademicYearId == AcademicYearId)
                .Select(s => new Semester
            {
                Name = s.Name,
                AcademicYearId=s.AcademicYearId,
                AcademicYear=new AcademicYear
                {
                    Name=s.AcademicYear.Name
                },
                Id = s.Id
            }).FirstOrDefaultAsync();
            if (existingSemester != null)
            {
                return Json($"Semester Name Is Used Before By {existingSemester.Name} {existingSemester.AcademicYear.Name} ..!");
            }

            return Json(true);
        }
    }
}
