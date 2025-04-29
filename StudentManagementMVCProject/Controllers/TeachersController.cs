using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Courses;
using StudentManagementMVCProject.ViewModels.Teachers;
using System.Text.Encodings.Web;
using System.Text;

namespace StudentManagementMVCProject.Controllers
{
    [Authorize]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentService _departmentService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public TeachersController( ITeacherService teacherService,ICourseService courseService,IUnitOfWork unitOfWork, IDepartmentService departmentService,UserManager<User> userManager,IEmailSender emailSender)
        {
            
            _teacherService = teacherService;
            _courseService = courseService;
            _unitOfWork = unitOfWork;
            _departmentService = departmentService;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Teachers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetTeachersToListAsync();
            return View(teachers);
        }

        // GET: Teachers/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var teacherUser = await _teacherService.GetByIdAsync(id);
            if (!User.IsInRole("Admin") && teacherUser?.UserId != currentUserId)
            {
                return Forbid();
            }

            var teacher = await _teacherService.GetTeacherDetailsByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code");

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddTeacherDTO model)
        {
            if (ModelState.IsValid)
            {
                var createdTeacher = await _teacherService.AddTeacherUserAsync(model);
                if (createdTeacher != null)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(createdTeacher.User);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = createdTeacher.UserId, code = code },
                    protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(createdTeacher.User.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    TempData["SweetAlertMessage"] = $"تم تسجيل المعلم <b>{createdTeacher.User.FirstName} {createdTeacher.User.LastName}</b> بنجاح";
                    TempData["SweetAlertType"] = "success";
                    TempData["SweetAlertButtonText"] = "متابعة";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["DepartmentId"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code", model.DepartmentId);


                if (!ModelState.ContainsKey("Email"))
                {
                    ModelState.AddModelError(string.Empty, "Failure in Creating Teacher");
                    return View(model);

                }
            }
            return View(model);

        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _teacherService.GetTeacherForEditByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["Departments"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code", teacher.DepartmentId);
            return View(teacher);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit( TeacherEditViewModel teacher)
        {
            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _teacherService.UpdateTeacherAsync(teacher);
                if (!success)
                {

                    ModelState.AddModelError(string.Empty, errorMessage);
                    
                    TempData["SweetAlertMessage"] = errorMessage;
                    TempData["SweetAlertType"] = "error";
                    TempData["SweetAlertButtonText"] = "متابعة";
                    ViewData["Departments"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code", teacher.DepartmentId);

                    return View(teacher);

                }
                TempData["SweetAlertMessage"] = $"Teacher {teacher.FirstName} {teacher.LastName} updated successfully.";
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertButtonText"] = "متابعة";


                return RedirectToAction(nameof(Details), new { id = teacher.Id });
            }
            ViewData["Departments"] = new SelectList(await _departmentService.GetDepartmentsNamesCodesAsync(), "Id", "Code", teacher.DepartmentId);
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var (success, errorMessage) = await _teacherService.DeleteTeacherAsync(id);
            if (!success)
            {
                return Json(new { success = false, error = errorMessage });
            }
            return Json(new { success = true, error = errorMessage });


        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Courses(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var teacherUser = await _teacherService.GetByIdAsync(id);
            if (!User.IsInRole("Admin") && teacherUser?.UserId != currentUserId)
            {
                return Forbid();
            }
            var teacher = await _teacherService.GetTeacherDetailsByIdAsync(id);
            if (teacher == null)
                return NotFound();
            ViewData["AcademicYears"] = new SelectList(
                             await _unitOfWork.Repository<AcademicYear>().GetAllAsync(),
                             "Id", "Name");


            return View(new TeacherCoursesViewModel
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.FullName,
                TeacherCourseEnrollments = new List<TeacherCourseEnrollmentsViewModel>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetTeacherCoursesPartial(int semesterId, int teacherId)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }
            var currentUserId = _userManager.GetUserId(User);
            var teacherUser = await _teacherService.GetByIdAsync(teacherId);
            if (!User.IsInRole("Admin") && teacherUser?.UserId != currentUserId)
            {
                return Forbid();
            }
            var courses = await _teacherService.GetTeacherCoursesBySemesterIdAsync(teacherId, semesterId);

            ViewBag.SemesterId = semesterId;
            return PartialView("_TeacherCoursesPartial", courses);
        }
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CourseStudents(int courseId,int semesterId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var teacherUser = await _teacherService.GetTeacherIdByUserIdAsync(currentUserId);
            var teacherCourseId = await _courseService.GetCourseTeacherIdBySmesterId(semesterId,courseId);
            if (!User.IsInRole("Admin") && (teacherUser?.UserId != currentUserId || teacherCourseId != teacherUser?.Id))
            {
                return Forbid();
            }
            //IMPLEMENT TEACHER ID TO PERVENT TO SHOW ANOTHER COURSES
            if (courseId == null || semesterId == null)
                return NotFound();
            var courseStudents = await _courseService.GetCourseStudentsListAsync(courseId, semesterId);
            ViewData["CourseName"] = _courseService.GetByIdAsync(courseId).Result.Name;
            ViewData["CourseId"] = courseId;
            ViewData["SemesterId"] = semesterId;
            ViewData["TeacherId"] = teacherCourseId;
            return View(courseStudents);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetStudentGrades(int studentId, int courseId, int semesterId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var teacherUser = await _teacherService.GetTeacherIdByUserIdAsync(currentUserId);
            var teacherCourseId = await _courseService.GetCourseTeacherIdBySmesterId(semesterId, courseId);
            if (!User.IsInRole("Admin") && (teacherUser?.UserId != currentUserId || teacherCourseId != teacherUser?.Id))
            {
                return Forbid();
            }
            var grades = await _courseService.GetStudentGradesAsync(studentId, courseId, semesterId);
            ViewData["SemesterId"] = semesterId;

            return Json(grades);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetEditGradesPartial(int studentId, int courseId, int semesterId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var teacherUser = await _teacherService.GetTeacherIdByUserIdAsync(currentUserId);
            var teacherCourseId = await _courseService.GetCourseTeacherIdBySmesterId(semesterId, courseId);
            if (!User.IsInRole("Admin") && (teacherUser?.UserId != currentUserId || teacherCourseId != teacherUser?.Id))
            {
                return Forbid();
            }
            var grades = await _courseService.GetStudentGradesAsync(studentId, courseId, semesterId);
            if (grades == null)
            {
                return NotFound();
            }
            return PartialView("_EditStudentGradesPartial", grades);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> EditStudentGrades(EditStudentGradesViewModel model)
        {
            var currentUserId = _userManager.GetUserId(User);
            var teacherUser = await _teacherService.GetTeacherIdByUserIdAsync(currentUserId);
            var teacherCourseId = await _courseService.GetCourseTeacherIdBySmesterId(model.SemesterId, model.CourseId);
            if (!User.IsInRole("Admin") && (teacherUser?.UserId != currentUserId || teacherCourseId != teacherUser?.Id))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var result = await _courseService.UpdateStudentGradesAsync(model);

            if (result.success)
            {
                return Json(new { success = true, message = result.errorMessage });
            }

            return Json(new { success = false, error = result.errorMessage });
        }

    }
}
