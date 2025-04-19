using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementMVCProject.Repositories.Implementations;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Registration;

namespace StudentManagementMVCProject.Controllers
{
    [Authorize(Roles ="Student")]
    public class CoursesRegistrationController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ISemesterService _semesterService;
        private readonly ICourseRegistrationService _courseRegistrationService;

        public CoursesRegistrationController(IStudentService studentService,ISemesterService semesterService,ICourseRegistrationService courseRegistrationService)
        {
            _studentService = studentService;
            _semesterService = semesterService;
            _courseRegistrationService = courseRegistrationService;
        }
        public async Task<IActionResult> Index()
        {
            
            var studentId =await _studentService.GetStudentIdByUserAsync(User.Identity.Name);
            
            var model = await _courseRegistrationService.GetRegistrationDetailsAsync(studentId);
            if (!model.Success)
            {
                TempData["ErrorMessage"] = model.Message;
            }
            return View(model.Data);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            var studentId = await _studentService.GetStudentIdByUserAsync(User.Identity.Name);
            var result = await _courseRegistrationService.RegisterCoursesAsync(studentId, model.SelectedCourseIds);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Confirmation", new
                {
                    courses = string.Join(",", result.RegisteredCourses.Select(c => c.Name))
                });
            }
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index");
        
        }
        [HttpGet]
        public IActionResult Confirmation(string courses)
        {
            var model = new RegistrationConfirmationViewModel
            {
                RegisteredCourses = courses?.Split(',').ToList() ?? new List<string>()
            };

            return View(model);
        }
    }
}
