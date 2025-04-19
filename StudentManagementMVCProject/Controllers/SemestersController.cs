using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Courses;

namespace StudentManagementMVCProject.Controllers
{
    public class SemestersController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly ICourseService _courseService;
        private readonly ITeacherService _teacherService;

        public SemestersController(IUnitOfWork unitofwork,ICourseService courseService, ITeacherService teacherService)
        {
            _unitofwork = unitofwork;
            _courseService = courseService;
            _teacherService = teacherService;
        }

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            var Semesters = await _unitofwork.Repository<Semester>().GetAsQueryAble().Include(s=>s.AcademicYear).ToListAsync();
            return View(Semesters);
        }

        // GET: Semesters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await _unitofwork.Repository<Semester>().GetAsQueryAble()
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // GET: Semesters/Create
        public async Task<IActionResult> Create()
        {
            var academicYearList = await _unitofwork.Repository<AcademicYear>().GetAllAsync();
            ViewData["AcademicYearId"] = new SelectList(academicYearList, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Semester semester)
        {
            if (ModelState.IsValid)
            {
                await _unitofwork.Repository<Semester>().AddAsync(semester);
                return RedirectToAction(nameof(Index));
            }
            var academicYearList = await _unitofwork.Repository<AcademicYear>().GetAllAsync();
            ViewData["AcademicYearId"] = new SelectList(academicYearList, "Id", "Name", semester.AcademicYearId);
            return View(semester);
        }

        // GET: Semesters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await _unitofwork.Repository<Semester>().GetAsQueryAble()
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (semester == null)
            {
                return NotFound();
            }
            var academicYearList = await _unitofwork.Repository<AcademicYear>().GetAllAsync();
            ViewData["AcademicYearId"] = new SelectList(academicYearList, "Id", "Name", semester.AcademicYearId);
            return View(semester);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Semester semester)
        {
            if (id != semester.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               
                    await _unitofwork.Repository<Semester>().UpdateAsync(semester);

                
                return RedirectToAction(nameof(Index));
            }
            var academicYearList = await _unitofwork.Repository<AcademicYear>().GetAllAsync();
            ViewData["AcademicYearId"] = new SelectList(academicYearList, "Id", "Name", semester.AcademicYearId);
            return View(semester);
        }

        // GET: Semesters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await _unitofwork.Repository<Semester>().GetAsQueryAble()
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // POST: Semesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var semester = await _unitofwork.Repository<Semester>().GetAsQueryAble()
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (semester != null)
            {
                await _unitofwork.Repository<Semester>().DeleteAsync(semester);

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddSemesterCourses(int id)
        {
            var semester = await _unitofwork.Repository<Semester>().GetAsQueryAble()
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(semester== null) { return NotFound(); }

            var existingCourses = await _unitofwork.Repository<CourseSchedule>()
                .GetAsQueryAble().Where(cs=>cs.SemesterId==id)
                .Include(cs=>cs.Teacher)
                .Include(cs=>cs.Course)
                .Select(cs=> new CourseTeacher { CourseId=cs.CourseId,
                TeacherId=cs.TeacherId,
                ScheduleId=cs.Id}).ToListAsync();

            ViewData["ExistingCourses"] = existingCourses;
            ViewData["Courses"] = await _courseService.GetCoursesDropDownListItemsAsync();
            ViewData["Teachers"] = await _teacherService.GetTeachersNameAsync();
            return View(semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSemesterCourses(int id, List<CourseTeacher> CourseTeacherPairs)
        {
            if (!ModelState.IsValid)
            {
                // إعادة تحميل البيانات في حالة وجود أخطاء
                var semester = await _unitofwork.Repository<Semester>()
                    .GetAsQueryAble()
                    .Include(s => s.AcademicYear)
                    .FirstOrDefaultAsync(m => m.Id == id);
                var existingCourses = await _unitofwork.Repository<CourseSchedule>()

                .GetAsQueryAble().Where(cs => cs.SemesterId == id)
                .Include(cs => cs.Teacher)
                .Include(cs => cs.Course)
                .Select(cs => new CourseTeacher
                {
                    CourseId = cs.CourseId,
                    TeacherId = cs.TeacherId,
                    ScheduleId = cs.Id
                }).ToListAsync();

                ViewData["ExistingCourses"] = existingCourses;

                ViewData["Courses"] = await _courseService.GetCoursesDropDownListItemsAsync();
                ViewData["Teachers"] = await _teacherService.GetTeachersNameAsync();
                return View(semester);
            }

            // جلب الجداول الزمنية الحالية
            var currentSchedules = await _unitofwork.Repository<CourseSchedule>()
                .GetAsQueryAble()
                .Where(cs => cs.SemesterId == id)
                .ToListAsync();

            // تحديد المقررات المحذوفة
            var submittedIds = CourseTeacherPairs.Where(p => p.ScheduleId > 0).Select(p => p.ScheduleId).ToHashSet();
            var toDelete = currentSchedules.Where(cs => !submittedIds.Contains(cs.Id)).ToList();

            // حذف المقررات المحذوفة
            foreach (var schedule in toDelete)
            {
                await _unitofwork.Repository<CourseSchedule>().DeleteAsync(schedule);
            }

            // معالجة المقررات المتبقية
            foreach (var pair in CourseTeacherPairs)
            {
                if (pair.ScheduleId > 0) // تحديث مقرر موجود
                {
                    var existing = currentSchedules.FirstOrDefault(cs => cs.Id == pair.ScheduleId);
                    if (existing != null)
                    {
                        existing.CourseId = pair.CourseId;
                        existing.TeacherId = pair.TeacherId;
                        await _unitofwork.Repository<CourseSchedule>().UpdateAsync(existing);
                    }
                }
                else // إضافة مقرر جديد
                {
                    var newSchedule = new CourseSchedule
                    {
                        CourseId = pair.CourseId,
                        TeacherId = pair.TeacherId,
                        SemesterId = id
                    };
                    await _unitofwork.Repository<CourseSchedule>().AddAsync(newSchedule);
                }
            }
            TempData["ToastType"] = "success"; // or "error", "info", "warning"
            TempData["ToastMessage"] = "تمت العملية بنجاح!";

            return RedirectToAction(nameof(Index));
        }

    }
}
