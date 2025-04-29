using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.DTOs.Students;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Shared;
using StudentManagementMVCProject.ViewModels.Students;
using System.Text;
using System.Text.Encodings.Web;

namespace StudentManagementMVCProject.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISemesterService _semesterService;
        private readonly IEmailSender _emailSender;

        public StudentsController( IStudentService studentService,UserManager<User> userManager, IMapper mapper,IDepartmentService departmentService,IUnitOfWork unitOfWork, ISemesterService semesterService,IEmailSender emailSender)
        {
            _studentService = studentService;
            _userManager = userManager;
            _mapper = mapper;
            _departmentService = departmentService;
            _unitOfWork = unitOfWork;
            _semesterService = semesterService;
            _emailSender = emailSender;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 4)
        {
            var (students,totalCount) = await _studentService.GetStudentsListAsync(pageNumber,pageSize);
            var viewModel = new StudentSearchViewModel
            {
                Students = new PagedResult<StudentListViewModel>
                {
                    Items = students,
                    TotalCount = totalCount,
                    PageNumber=pageNumber,
                    PageSize=pageSize
                },


                Filter = new StudentSearchFilterViewModel()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }
            };
            return View(viewModel);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchStudents(StudentSearchFilterViewModel searchModel)
        {
            searchModel.PageNumber = searchModel.PageNumber <= 0 ? 1 : searchModel.PageNumber;
            searchModel.PageSize = searchModel.PageSize <= 0 ? 12 : searchModel.PageSize;

            var (students, totalCount) = await _studentService.GetFilteredStudentsListAsync(searchModel);
            var viewModel = new StudentSearchViewModel
            {
                Students = new PagedResult<StudentListViewModel>
                {
                    Items = students,
                    TotalCount = totalCount,
                    PageNumber = searchModel.PageNumber,
                    PageSize = searchModel.PageSize
                },
                Filter = searchModel
            };
            ViewData["TotalCount"] = totalCount;
            ViewData["PageNumber"] = searchModel.PageNumber;
            ViewData["PageSize"] = searchModel.PageSize;
            return PartialView("_StudentsCards", viewModel.Students.Items);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var Depts=await _departmentService.GetDepartmentsNamesCodesAsync();

            var model = new AddStudentDTO
            {
                Departments = new SelectList(Depts, "Id", "Code"),
                StudentStatus = new SelectList(
                    Enum.GetValues(typeof(StudentStatus))
                        .Cast<StudentStatus>()
                        .Select(s => new { Value = (int)s, Text = s.ToString() }),
                    "Value",
                    "Text"
                ),
                AcademicLevel = new SelectList(
                    Enum.GetValues(typeof(AcademicLevel))
                        .Cast<AcademicLevel>()
                        .Select(s => new { Value = (int)s, Text = s.ToString() }),
                    "Value",
                    "Text")
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddStudentDTO model)
        {
            var Depts = await _departmentService.GetDepartmentsNamesCodesAsync();
            if (ModelState.IsValid)
            {

                var createdStudent = await _studentService.AddStudentUserAsync(model);



                if (createdStudent != null)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(createdStudent.User);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = createdStudent.UserId, code = code },
                    protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(createdStudent.User.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    TempData["SweetAlertMessage"] = $"تم تسجيل الطالب <b>{createdStudent.User.FirstName} {createdStudent.User.LastName}</b> بنجاح";
                    TempData["SweetAlertType"] = "success";
                    TempData["SweetAlertButtonText"] = "متابعة";
                    //TempData["ToastType"] = "success";
                    //TempData["ToastMessage"] = "Student Created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                model.Departments = new SelectList(Depts, "Id", "Code", model.DepartmentId);
                model.StudentStatus = new SelectList(
                    Enum.GetValues(typeof(StudentStatus))
                        .Cast<StudentStatus>()
                        .Select(s => new { Value = (int)s, Text = s.ToString() }),
                    "Value",
                    "Text"
                );
                model.AcademicLevel = new SelectList(
                    Enum.GetValues(typeof(AcademicLevel))
                        .Cast<AcademicLevel>()
                        .Select(s => new { Value = (int)s, Text = s.ToString() }),
                    "Value",
                    "Text");
                
                if (!ModelState.ContainsKey("Email"))
                {
                    ModelState.AddModelError(string.Empty, "Failure in Creating Student");
                    TempData["SweetAlertMessage"] = $"Failure in Creating Student";
                    TempData["SweetAlertType"] = "error";
                    TempData["SweetAlertButtonText"] = "متابعة";
                    return View(model);

                }

            }
            model.Departments = new SelectList(Depts, "Id", "Code", model.DepartmentId);
            model.StudentStatus = new SelectList(
                Enum.GetValues(typeof(StudentStatus))
                    .Cast<StudentStatus>()
                    .Select(s => new { Value = (int)s, Text = s.ToString() }),
                "Value",
                "Text"
            );
            model.AcademicLevel = new SelectList(
                Enum.GetValues(typeof(AcademicLevel))
                    .Cast<AcademicLevel>()
                    .Select(s => new { Value = (int)s, Text = s.ToString() }),
                "Value",
                "Text");
            return View(model);
        }
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var student = await _studentService.GetByIdAsync(id);
            if (!User.IsInRole("Admin") && student?.UserId != currentUserId)
            {
                return Forbid();
            }

            var Student= await _studentService.GetStudentDetailsAsync(id);
            if (Student == null)
                return NotFound();
            return View(Student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Student Not Found.";
                return Json(new { success = false, error = "الطالب غير موجود" });
            }

            var studentUser = await _userManager.FindByIdAsync(student.UserId);
            if (studentUser == null)
            {
                return Json(new { success = false, error = "المستخدم المرتبط بالطالب غير موجود" });
            }

            using (var trans = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _studentService.DeleteAsync(student);
                    var result = await _userManager.DeleteAsync(studentUser);
                    if (!result.Succeeded)
                    {
                        await trans.RollbackAsync();
                        return Json(new { success = false, error = "فشل حذف المستخدم المرتبط" });
                    }

                    await _unitOfWork.CompleteAsync();
                    await trans.CommitAsync();
                    
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    
                    return Json(new { success = false, error = $"حدث خطأ أثناء الحذف: {ex.Message}" });
                }
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentForEditAsync(id);
            if(student == null)
                return NotFound();
            var model = _mapper.Map<StudentEditViewModel>(student);
            ViewData["Departments"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code", model.DepartmentId);
            ViewData["StudentStatus"] = new SelectList(
                    Enum.GetValues(typeof(StudentStatus))
                        .Cast<StudentStatus>()
                        .Select(s => new { Value = (int)s, Text = s.ToString() }),
                    "Value",
                    "Text"
                );
            ViewData["AcademicLevel"]= new SelectList(
                    Enum.GetValues(typeof(AcademicLevel))
                        .Cast<AcademicLevel>()
                        .Select(s => new { Value = (int)s, Text = s.ToString() }),
                    "Value",
                    "Text");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(StudentEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Departments"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code", model.DepartmentId);
                ViewData["StudentStatus"] = new SelectList(
                        Enum.GetValues(typeof(StudentStatus))
                            .Cast<StudentStatus>()
                            .Select(s => new { Value = (int)s, Text = s.ToString() }),
                        "Value",
                        "Text"
                    );
                ViewData["AcademicLevel"] = new SelectList(
                        Enum.GetValues(typeof(AcademicLevel))
                            .Cast<AcademicLevel>()
                            .Select(s => new { Value = (int)s, Text = s.ToString() }),
                        "Value",
                        "Text");
                return View(model);
            }
            var (success, errorMessage) = await _studentService.UpdateStudentAsync(model);
            if (!success)
            {

                ModelState.AddModelError(string.Empty, errorMessage);
                //TempData["ToastType"] = "error";
                //TempData["ToastMessage"] = errorMessage;
                TempData["SweetAlertMessage"] = errorMessage;
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertButtonText"] = "متابعة";
                ViewData["Departments"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code", model.DepartmentId);
                ViewData["StudentStatus"] = new SelectList(
                        Enum.GetValues(typeof(StudentStatus))
                            .Cast<StudentStatus>()
                            .Select(s => new { Value = (int)s, Text = s.ToString() }),
                        "Value",
                        "Text"
                    );
                ViewData["AcademicLevel"] = new SelectList(
                        Enum.GetValues(typeof(AcademicLevel))
                            .Cast<AcademicLevel>()
                            .Select(s => new { Value = (int)s, Text = s.ToString() }),
                        "Value",
                        "Text");
                return View(model);

            }
            TempData["SweetAlertMessage"] = $"Student updated successfully.";
            TempData["SweetAlertType"] = "success";
            TempData["SweetAlertButtonText"] = "متابعة";

            
            return RedirectToAction(nameof(Details),new {id=model.Id});
        }
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Courses(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var student = await _studentService.GetByIdAsync(id);

            if (!User.IsInRole("Admin") && student?.UserId != currentUserId)
            {
                return Forbid();
            }
            var Student = await _studentService.GetStudentDetailsAsync(id);
            if (Student == null)
                return NotFound();

            ViewData["AcademicYears"] = new SelectList(
                await _unitOfWork.Repository<AcademicYear>().GetAllAsync(),
                "Id", "Name");
            ViewData["StudentId"] = Student.Id;
            ViewData["StudentName"] = Student.FullName;
            ViewData["Enrollments"] = new List<StudentCourseEnrollment>();

            return View();
        }


        
        [HttpPost]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> GetCourses(int studentId, int semesterId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var student = await _studentService.GetStudentIdByUserIdAsync(currentUserId);

            if (!User.IsInRole("Admin") && student?.UserId != currentUserId)
            {
                return Forbid();
            }
            var studentCourses = await _studentService.GetStudentCoursesBySemesterIdAsync(studentId, semesterId);
            if (studentCourses == null)
                return NotFound();

            return PartialView("_StudentCourses", studentCourses.CoursesEnrollment);
        }

        

    }
}
