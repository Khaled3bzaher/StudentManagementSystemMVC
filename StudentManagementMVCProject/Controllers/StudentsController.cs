using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.DTOs.Students;
using StudentManagementMVCProject.Enums;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Students;

namespace StudentManagementMVCProject.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISemesterService _semesterService;

        public StudentsController( IStudentService studentService,UserManager<User> userManager, IMapper mapper,IDepartmentService departmentService,IUnitOfWork unitOfWork, ISemesterService semesterService)
        {
            _studentService = studentService;
            _userManager = userManager;
            _mapper = mapper;
            _departmentService = departmentService;
            _unitOfWork = unitOfWork;
            _semesterService = semesterService;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetStudentsListAsync();
            var viewModel = new StudentSearchViewModel
            {
                Students = students,
                Filter= new StudentSearchFilterViewModel()
            };
            return View(viewModel);
        }
        public async Task<IActionResult> SearchStudents(StudentSearchFilterViewModel searchModel)
        {
            var searchedStudents = await _studentService.GetFilteredStudentsListAsync(searchModel);

            return PartialView("_StudentsCards", searchedStudents);
        }
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
        public async Task<IActionResult> Create(AddStudentDTO model)
        {
            var Depts = await _departmentService.GetDepartmentsNamesCodesAsync();
            if (ModelState.IsValid)
            {
                var createdStudent = await _studentService.AddStudentUserAsync(model);
                if (createdStudent != null)
                {
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

        public async Task<IActionResult> Details(int id)
        {
            var Student= await _studentService.GetStudentDetailsAsync(id);
            if (Student == null)
                return NotFound();
            return View(Student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> Courses(int id)
        {
            var student = await _studentService.GetStudentDetailsAsync(id);
            if (student == null)
                return NotFound();

            ViewData["AcademicYears"] = new SelectList(
                await _unitOfWork.Repository<AcademicYear>().GetAllAsync(),
                "Id", "Name");
            ViewData["StudentId"] = student.Id;
            ViewData["StudentName"] = student.FullName;
            ViewData["Enrollments"] = new List<StudentCourseEnrollment>();

            return View();
        }


        public async Task<IActionResult> GetSemestersByAcademicYearId(int academicYearId)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }

            var semesters = await _semesterService.GetSemestersByAcademicYearAsync(academicYearId);
            return Json(semesters.Select(s => new { s.Id, s.Name }));
        }
        [HttpPost]
        public async Task<IActionResult> GetCourses(int studentId, int semesterId)
        {
            var studentCourses = await _studentService.GetStudentCoursesBySemesterIdAsync(studentId, semesterId);
            if (studentCourses == null)
                return NotFound();

            return PartialView("_StudentCourses", studentCourses.CoursesEnrollment);
        }

        
    }
}
