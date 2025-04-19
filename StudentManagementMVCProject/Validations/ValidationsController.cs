using Microsoft.AspNetCore.Authorization;
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
        //REMOTE VALIDATIONS
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyNationalId(string nationalId, string email = null)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
            // 1. التحقق من عدم الفراغ
            if (string.IsNullOrEmpty(nationalId))
            {
                return Json("National Id is Required .. !");
            }

            // 2. التحقق من الطول (14 رقم)
            if (nationalId.Length != 14)
            {
                return Json("National Id Length Must be 14 Digits ..!");
            }

            // 3. التحقق من أن كل الأحرف أرقام
            if (!nationalId.All(char.IsDigit))
            {
                return Json("National Id Must be Numbers Only ..!");
            }

            // 4. التحقق من صحة الرقم القومي (خوارزمية التحقق)
            if (!IsValidEgyptianNationalId(nationalId))
            {
                return Json("Egyption National Id is Not Valid ..!");
            }

            // 5. التحقق من عدم تكرار الرقم في قاعدة البيانات (باستثناء السجل الحالي)
            var existingPerson = await _unitOfWork.Repository<User>().GetAsQueryAble().Select(u=>new User
            {
                FirstName=u.FirstName,
                LastName=u.LastName,
                NationalId=u.NationalId,
                Email=u.Email
            }).FirstOrDefaultAsync(u => u.NationalId == nationalId && u.Email != email);

            if (existingPerson != null)
            {
                return Json($"National Id Exists Before By {existingPerson.FirstName} {existingPerson.LastName} ..!");
            }

            return Json(true);
        }
        private bool IsValidEgyptianNationalId(string nationalId)
        {
            // خوارزمية التحقق من صحة الرقم القومي المصري
            if (nationalId.Length != 14) return false;

            // التحقق من صحة تاريخ الميلاد المضمن في الرقم
            int year = int.Parse(nationalId.Substring(1, 2));
            int month = int.Parse(nationalId.Substring(3, 2));
            int day = int.Parse(nationalId.Substring(5, 2));

            if (month < 1 || month > 12) return false;
            if (day < 1 || day > 31) return false;

            // التحقق من الرقم الأول (القرن)
            char centuryDigit = nationalId[0];
            if (centuryDigit != '2' && centuryDigit != '3') return false;

            // يمكن إضافة المزيد من قواعد التحقق حسب الحاجة

            return true;
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyNumber(string phoneNumber, string email = null)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Json("Phone Number is Required .. !");
            }


            // 3. التحقق من أن كل الأحرف أرقام
            if (!phoneNumber.All(char.IsDigit))
            {
                return Json("Phone Number Must be Numbers Only ..!");
            }

            if (!phoneNumber.StartsWith("01"))
            {
                return Json("Phone Number Must Starts with 01 ..!");
            }
            

            // 2. التحقق من الطول (14 رقم)
            if (phoneNumber.Length != 11)
            {
                return Json("Phone Number Length Must be 11 Digits ..!");
            }

            var secondDigit = phoneNumber[2];
            var validSecondDigits = new[] { '0', '1', '2', '5' }; // 010, 011, 012, 015
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
        public async Task<IActionResult> VerifyParentNumber(string parentPhone)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
            


            // 3. التحقق من أن كل الأحرف أرقام
            if (!parentPhone.All(char.IsDigit))
            {
                return Json("Parent Number Must be Numbers Only ..!");
            }

            if (!parentPhone.StartsWith("01"))
            {
                return Json("Parent Number Must Starts with 01 ..!");
            }
            

            // 2. التحقق من الطول (14 رقم)
            if (parentPhone.Length != 11)
            {
                return Json("Parent Number Length Must be 11 Digits ..!");
            }

            var secondDigit = parentPhone[2];
            var validSecondDigits = new[] { '0', '1', '2', '5' }; // 010, 011, 012, 015
            if (!validSecondDigits.Contains(secondDigit))
            {
                return Json("Parent Number Must Follow Egyptian Communcation Providers ..!");

            }

            return Json(true);

        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyStudentNumber(string studentNumber,int Id)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
            if (!studentNumber.All(char.IsDigit))
            {
                return Json("Student Number Must be Numbers Only ..!");
            }
            if (!(studentNumber.Length >= 11 && studentNumber.Length <= 14))
            {
                return Json("Student Number Length Must be 11 to 14 Digits ..!");
            }
            var existingPerson = await _unitOfWork.Repository<Student>().GetAsQueryAble().Include(s=>s.User).Select(s=> new
            {
                StudentNumber= s.StudentNumber,
                Id=s.Id,
                User =new User
                {
                    FirstName=s.User.FirstName,
                    LastName=s.User.LastName
                }
            }).FirstOrDefaultAsync(s => s.StudentNumber == studentNumber && s.Id != Id);

            if (existingPerson != null)
            {
                return Json($"National Id Exists Before By {existingPerson.User.FirstName} {existingPerson.User.LastName} ..!");
            }

            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(email))
            {
                return Json("Email is Required .. !");
            }
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                return Json("Enter Valid Email Please ..!");
            }
            var existingPerson = await _unitOfWork.Repository<User>().GetAsQueryAble().Select(s => new User
            {
                Email = s.Email,
                FirstName = s.FirstName,
                LastName = s.LastName
            }).FirstOrDefaultAsync(s => s.Email == email );
            if (existingPerson != null)
            {
                return Json($"Email Is Used Before By {existingPerson.FirstName} {existingPerson.LastName} ..!");
            }
            return Json(true);

        }
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyBirthDay(DateTime DateOfBirth)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
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
        public async Task<IActionResult> VerifyName(string firstName = null,string lastName=null,string Name=null)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
            string name = null;
            if (!string.IsNullOrEmpty(firstName))
            {
                name = firstName;
            }else if (!string.IsNullOrEmpty(lastName))
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
            var regex = new Regex(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z\s]+$");
            if (!regex.IsMatch(name))
            {
                return Json("Name Must be Characters Only ..!");
            }
            if (!(name.Length >= 4 && name.Length < 50))
            {
                return Json("Name Must be 4 to 50 Digits ..!");
            }
            return Json(true);

        }
    }
}
