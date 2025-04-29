using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Implementations;
using StudentManagementMVCProject.Repositories.Interfaces;

namespace StudentManagementMVCProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISemesterService _semesterService;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ISemesterService semesterService,IUnitOfWork unitOfWork)
        {
            _semesterService = semesterService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var currentSemester = await _semesterService.GetCurrentSemesterAsync();
            var studentCount = await _unitOfWork.Repository<Student>().GetCountAsync();
            var teacherCount = await _unitOfWork.Repository<Teacher>().GetCountAsync();
            var courseCount = await _unitOfWork.Repository<Course>().GetCountAsync();
            var departmentCount = await _unitOfWork.Repository<Department>().GetCountAsync();

            var currentSemesterName = currentSemester?.Name ?? "غير محدد";
            var semesterStartDate = currentSemester?.StartDate.ToString("yyyy-MM-dd") ?? null;
            var semesterEndDate = currentSemester?.EndDate.ToString("yyyy-MM-dd") ?? null;

            ViewData["StudentCount"] = studentCount;
            ViewData["TeacherCount"] = teacherCount;
            ViewData["CourseCount"] = courseCount;
            ViewData["DepartmentCount"] = departmentCount;
            ViewData["CurrentSemesterName"] = currentSemesterName;
            ViewData["SemesterStartDate"] = semesterStartDate;
            ViewData["SemesterEndDate"] = semesterEndDate;
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }
    }
}
