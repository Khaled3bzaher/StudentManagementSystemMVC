using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Interfaces;

namespace StudentManagementMVCProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class CoursesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CoursesController(IUnitOfWork unitOfWork, ICourseService courseService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _courseService = courseService;
            _mapper = mapper;
        }
        

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsyncOptimized();

            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetCourseByIdAsyncOptimized(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public async Task<IActionResult> Create()
        {
            var Depts = await _unitOfWork.Repository<Department>().GetAsQueryAble().Select(src=>new {Id=src.Id,Name=src.Name}).ToListAsync();
            var Courses = await _courseService.GetCoursesDropDownListItemsAsync();
            ViewData["DepartmentId"] = new SelectList(Depts, "Id", "Name");
            ViewData["PrerequisiteCourseId"] = new SelectList(Courses, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            var Depts = await _unitOfWork.Repository<Department>().GetAsQueryAble().Select(src => new { Id = src.Id, Name = src.Name }).ToListAsync();
            var Courses = await _courseService.GetCoursesDropDownListItemsAsync();

            ViewData["DepartmentId"] = new SelectList(Depts, "Id", "Name", course.DepartmentId);
            ViewData["PrerequisiteCourseId"] = new SelectList(Courses, "Id", "Name", course.PrerequisiteCourseId);

            if (ModelState.IsValid)
            {
                
                await _unitOfWork.Repository<Course>().AddAsync(course);
                
                TempData["SweetAlertMessage"] = $"Course {course.Code} {course.Name} is Created..!";
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertButtonText"] = "متابعة";
                return RedirectToAction(nameof(Index));
            }

           
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var Depts = await _unitOfWork.Repository<Department>().GetAsQueryAble().Select(src => new { Id = src.Id, Name = src.Name }).ToListAsync();
            var CoursesExcludeCurrentCourse = await _courseService.GetCoursesDropDownListItemsExcludeCurrentCourseAsync(id);
            ViewData["DepartmentId"] = new SelectList(Depts, "Id", "Name", course.DepartmentId);
            ViewData["PrerequisiteCourseId"] = new SelectList(CoursesExcludeCurrentCourse, "Id", "Name", course.PrerequisiteCourseId);
            return View(course);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course course)
        {
            
            var Depts = await _unitOfWork.Repository<Department>().GetAsQueryAble().Select(src => new { Id = src.Id, Name = src.Name }).ToListAsync();
            var CoursesExcludeCurrentCourse = await _courseService.GetCoursesDropDownListItemsExcludeCurrentCourseAsync(course.Id);
            ViewData["DepartmentId"] = new SelectList(Depts, "Id", "Name", course.DepartmentId);
            ViewData["PrerequisiteCourseId"] = new SelectList(CoursesExcludeCurrentCourse, "Id", "Name", course.PrerequisiteCourseId);

            if (ModelState.IsValid)
            {
                
                await _unitOfWork.Repository<Course>().UpdateAsync(course);

                TempData["SweetAlertMessage"] = $"Course {course.Code} {course.Name} Edited Successfully ..!";
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertButtonText"] = "متابعة";

                return RedirectToAction(nameof(Index));
            }
           return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetCourseByIdAsyncOptimized(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseService.GetCourseByIdAsyncOptimized(id);


            if (course != null)
            {
                var DependentCourse = await _courseService.GetCourseDependent(id);
                if (DependentCourse is not null)
                {
                    TempData["SweetAlertMessage"] = $"Error: I Can't Remove Prerequisite Course .. Change Dependent from Course {DependentCourse.Name} First.!";
                    TempData["SweetAlertType"] = "error";
                    TempData["SweetAlertButtonText"] = "متابعة";


                    return View(course);
                }
                var deletedCourse = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
                await _unitOfWork.Repository<Course>().DeleteAsync(deletedCourse);
               
                TempData["SweetAlertMessage"] = $"Course {deletedCourse.Name} Deleted.!";
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertButtonText"] = "متابعة";
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
