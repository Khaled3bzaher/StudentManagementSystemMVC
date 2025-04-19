using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.DTOs.Departments;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Departments;

namespace StudentManagementMVCProject.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly ITeacherService _teacherService;
        private readonly IMapper _mapper;

        public DepartmentsController(ApplicationDbContext context, IDepartmentService departmentService, ITeacherService teacherService , IMapper mapper)
        {
            _departmentService = departmentService;
            _teacherService = teacherService;
            _mapper = mapper;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var Departments = await _departmentService.GetDepartmentWithHeadsAsync();
            return View(Departments);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public async Task<IActionResult> Create()
        {
            var TeachersName=await _teacherService.GetTeachersNameAsync();
            ViewData["HeadTeacherID"] = new SelectList(TeachersName, "Id", "FullName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( AddDepartmentDTO model)
        {
            if (ModelState.IsValid)
            {
                var Department=_mapper.Map<Department>(model);
                await _departmentService.AddAsync(Department);
                return RedirectToAction(nameof(Index));
            }
            var TeachersName = await _teacherService.GetTeachersNameAsync();
            ViewData["HeadTeacherID"] = new SelectList(TeachersName, "Id", "FullName", model.HeadTeacherID);
            return View(model);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetDepartmentByIdForEditAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            var TeachersNameInSameDepartment = await _departmentService.GetDepartmentTeachersByIdAsync(id);
            
            ViewData["HeadTeacherID"] = new SelectList(TeachersNameInSameDepartment, "Id", "FullName", department.HeadTeacherID);
            return View(department);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditDepartmentViewModel model)
        {
            

            if (ModelState.IsValid)
            {
                var Department = _mapper.Map<Department>(model);
                await _departmentService.UpdateAsync(Department);
                return RedirectToAction(nameof(Index));
            }
            var TeachersNameInSameDepartment = await _departmentService.GetDepartmentTeachersByIdAsync(model.Id);

            ViewData["HeadTeacherID"] = new SelectList(TeachersNameInSameDepartment, "Id", "FullName", model.HeadTeacherID);
            return View(model);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _departmentService.GetDepartmentByIdAsync(id);

            if (model != null)
            {
                if (model.DepartmentHeadName != " ")
                {
                    TempData["StatusMessage"] = $"Error: Can't Delete {model.Name} Department Because it's related.!";
                    return View(model);
                }
                var Department = _mapper.Map<Department>(model);
                await _departmentService.DeleteAsync(Department);
            }
            TempData["StatusMessage"] = $"Department {model.Name} Department is Deleted.!";

            return RedirectToAction(nameof(Index));
        }


    }
}
