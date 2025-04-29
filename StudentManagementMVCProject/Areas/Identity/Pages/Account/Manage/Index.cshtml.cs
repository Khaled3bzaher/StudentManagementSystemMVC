// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Models;
using Microsoft.AspNetCore.Hosting;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Validations;

namespace StudentManagementMVCProject.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment webHostEnvironment,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Phone number")]
            [PhoneNumberValidation]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Image")]
            public IFormFile ProfileImage { get; set; }

            public string CurrentProfileImagePath { get; set; }
            [MaxLength(100, ErrorMessage = "Max Length For Address is 100 Letters")]
            public string? Address { get; set; }
            //[Remote(action: "VerifyParentNumber", controller: "Validations")]
            [Display(Name = "Parent Phone")]
            [ParentNumberValidation]
            public string? ParentPhone { get; set; }
            [MaxLength(50, ErrorMessage = "Max Length For Qualification is 50 Letters")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Qualification must contain letters only.")]
            public string? Qualification { get; set; }
            public string? Role { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                CurrentProfileImagePath = user.ProfilePictureURL,
                Role = userRoles.FirstOrDefault() // Assuming only one role per user
            };

            // If the user is a Student
            if (Input.Role == "Student")
            {
                var student = await _unitOfWork.Repository<Student>().GetAsQueryAble().Where(s => s.UserId == user.Id).FirstOrDefaultAsync();

                if (student != null)
                {
                    Input.Address = student.Address;
                    Input.ParentPhone = student.ParentPhone;
                }
            }
            // If the user is a Teacher
            else if (Input.Role == "Teacher")
            {
                var teacher = await _unitOfWork.Repository<Teacher>().GetAsQueryAble().Where(t => t.UserId == user.Id).FirstOrDefaultAsync();
                if (teacher != null)
                {
                    Input.Qualification = teacher.Qualification;
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var existingUserWithPhone = await _userManager.Users
                .AnyAsync(u => u.PhoneNumber == Input.PhoneNumber && u.Id != user.Id);

            if (existingUserWithPhone)
            {
                StatusMessage = "Error: Number is already in use.";

                ModelState.AddModelError("Input.PhoneNumber", "Number is already in use.");
                await LoadAsync(user);
                return Page();
            }

            if (Input.PhoneNumber != user.PhoneNumber)
            {

                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.ProfileImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(Input.ProfileImage.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Input.ProfileImage", "Only image formats (jpg, png) are allowed.");
                    await LoadAsync(user);
                    return Page();
                }
                if (Input.ProfileImage.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("Input.ProfileImage", "Maximum allowed size is 2MB.");
                    await LoadAsync(user);
                    return Page();
                }
                if (!string.IsNullOrEmpty(user.ProfilePictureURL))
                {
                    var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePictureURL.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                var uploadsFolder = Path.Combine("images", "profiles");
                var fileName = Guid.NewGuid().ToString() + extension;
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder, fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await Input.ProfileImage.CopyToAsync(stream);
                }

                user.ProfilePictureURL = "/" + Path.Combine(uploadsFolder, fileName).Replace("\\", "/");
            }

            if (Input.Role == "Student")
            {
                var student = await _unitOfWork.Repository<Student>().GetAsQueryAble().Where(s => s.UserId == user.Id).FirstOrDefaultAsync();
                if (student != null)
                {
                    student.Address = Input.Address;
                    student.ParentPhone = Input.ParentPhone;
                    await _unitOfWork.Repository<Student>().UpdateAsync(student);
                }
            }
            else if (Input.Role == "Teacher")
            {
                var teacher = await _unitOfWork.Repository<Teacher>().GetAsQueryAble().Where(t => t.UserId == user.Id).FirstOrDefaultAsync();
                if (teacher != null)
                {
                    if (string.IsNullOrEmpty(Input.Qualification))
                    {
                        ModelState.AddModelError("Input.Qualification", "Qualification Must be Not Empty");
                        await LoadAsync(user);
                        return Page();
                    }
                    teacher.Qualification = Input.Qualification;
                    await _unitOfWork.Repository<Teacher>().UpdateAsync(teacher);
                }

            }


            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
