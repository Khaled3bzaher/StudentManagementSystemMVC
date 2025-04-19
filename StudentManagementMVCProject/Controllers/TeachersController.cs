using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.DTOs.Teachers;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Implementations;
using StudentManagementMVCProject.Repositories.Interfaces;
using StudentManagementMVCProject.ViewModels.Teachers;

namespace StudentManagementMVCProject.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITeacherService _teacherService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public TeachersController(IUnitOfWork unitOfWork, ITeacherService teacherService, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _teacherService = teacherService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var teachers = await _unitOfWork.Repository<Teacher>().GetAsQueryAble()
                .Include(s => s.User).Include(s => s.Department)
                .ProjectTo<TeacherListViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return View(teachers);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Department)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public async Task<IActionResult> Create()
        {
            var Depts = await _unitOfWork.Repository<Department>().GetAllAsync();

            ViewData["DepartmentId"] = new SelectList(Depts, "Id", "Name");
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddTeacherDTO model)
        {
            if (ModelState.IsValid)
            {
                var createdTeacher = await _teacherService.AddTeacherUserAsync(model);
                if (createdTeacher != null)
                {
                    return RedirectToAction(nameof(Index));
                }

                var isExistBefore = await _userManager.FindByEmailAsync(model.Email);
                var Depts = await _unitOfWork.Repository<Department>().GetAllAsync();
                ViewData["DepartmentId"] = new SelectList(Depts, "Id", "Name", model.DepartmentId);
                if (isExistBefore != null)
                {
                    ModelState.AddModelError("Email", "Email is Used Before..!");
                    return View(model);
                }
                if (!ModelState.ContainsKey("Email"))
                {
                    ModelState.AddModelError(string.Empty, "Failure in Creating Student");
                    return View(model);

                }
            }
            return View(model);

        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Code", teacher.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", teacher.UserId);
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,DepartmentId,Qualification,HireDate")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Code", teacher.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", teacher.UserId);
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _unitOfWork.Repository<Teacher>().GetAsQueryAble()
                .Include(t => t.Department)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(id);
            if (teacher != null)
            {
                var teacherUser = await _userManager.FindByIdAsync(teacher.UserId);
                await _unitOfWork.Repository<Teacher>().DeleteAsync(teacher);
                var result = await _userManager.DeleteAsync(teacherUser);

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
