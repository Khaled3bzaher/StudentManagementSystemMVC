using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;

namespace StudentManagementMVCProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AcademicYearsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AcademicYearsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: AcademicYears
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.Repository<AcademicYear>().GetAllAsync());
        }

        

        // GET: AcademicYears/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AcademicYears/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] AcademicYear academicYear)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Repository<AcademicYear>().AddAsync(academicYear);
                TempData["SweetAlertMessage"] = $"AcademicYear {academicYear.Name} is Created..!";
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertButtonText"] = "متابعة";
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }

        // GET: AcademicYears/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicYear = await _unitOfWork.Repository<AcademicYear>().GetByIdAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }
            return View(academicYear);
        }

        // POST: AcademicYears/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] AcademicYear academicYear)
        {
            if (id != academicYear.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _unitOfWork.Repository<AcademicYear>().UpdateAsync(academicYear);
                    TempData["SweetAlertMessage"] = $"AcademicYear {academicYear.Name} is Updated..!";
                    TempData["SweetAlertType"] = "success";
                    TempData["SweetAlertButtonText"] = "متابعة";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicYearExists(academicYear.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(academicYear);
        }

        // GET: AcademicYears/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicYear = await  _unitOfWork.Repository<AcademicYear>().GetByIdAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }

            return View(academicYear);
        }

        // POST: AcademicYears/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicYear = await _unitOfWork.Repository<AcademicYear>().GetByIdAsync(id);
            if (academicYear != null)
            {
                TempData["SweetAlertMessage"] = $"AcademicYear {academicYear.Name} is Deleted..!";
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertButtonText"] = "متابعة";
                await _unitOfWork.Repository<AcademicYear>().DeleteAsync(academicYear);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AcademicYearExists(int id)
        {
            return _unitOfWork.Repository<AcademicYear>().GetAsQueryAble().Any(e => e.Id == id);
        }
    }
}
