using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.ViewModels.Roles;

namespace StudentManagementMVCProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public RoleController(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper=mapper;
        }
        public IActionResult Index()
        {
            var Roles = _roleManager.Roles.ToList();
            var results=_mapper.Map<List<GetRoleViewModel>>(Roles);
            return View(results);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(AddRoleViewModel model) {

            if (ModelState.IsValid)
            {
                var isExistRole = await _roleManager.Roles.FirstOrDefaultAsync(r=>r.Name.ToLower()==model.Name.ToLower());
                if (isExistRole is not null)
                {
                    ModelState.AddModelError("Name", "Role is Exists Before");
                    return View(model);

                }
                var role = new IdentityRole() { Name = model.Name };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    TempData["ToastType"] = "success"; // or "error", "info", "warning"
                    TempData["ToastMessage"] = "تمت العملية بنجاح!";
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            return View(model);

        }
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(string id) { 
            var role= await _roleManager.FindByIdAsync(id);
            if(role==null) return NotFound();
            var result= await _roleManager.DeleteAsync(role);
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            var result=_mapper.Map<GetRoleViewModel>(role);
            return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(GetRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null) return NotFound();
                var isExistRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == model.Name.ToLower());
                if (isExistRole is not null)
                {
                    ModelState.AddModelError("Name", "Role is Exists Before");
                    return View(model);
                }
                var updatedRole = _mapper.Map(model, role);
                var result = await _roleManager.UpdateAsync(updatedRole);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            return View(model);
        }
        }
    }
