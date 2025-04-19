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
            var studentCount =  _unitOfWork.Repository<Student>().GetAllAsync().Result.Count();
            var teacherCount = _unitOfWork.Repository<Teacher>().GetAllAsync().Result.Count();
            var courseCount = _unitOfWork.Repository<Course>().GetAllAsync().Result.Count();
            var departmentCount = _unitOfWork.Repository<Department>().GetAllAsync().Result.Count();

            // تخزين باقي البيانات في ViewData
            var currentSemesterName = currentSemester?.Name ?? "غير محدد";
            var semesterStartDate = currentSemester?.StartDate.ToString("dd/MM/yyyy") ?? "غير محدد";
            var semesterEndDate = currentSemester?.EndDate.ToString("dd/MM/yyyy") ?? "غير محدد";

            // تم تمرير القيم إلى ال View
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
