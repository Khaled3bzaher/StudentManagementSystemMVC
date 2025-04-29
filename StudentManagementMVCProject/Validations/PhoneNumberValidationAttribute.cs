using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Data;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.Validations
{
    public class PhoneNumberValidationAttribute : ValidationAttribute,IClientModelValidator
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
            var httpContext = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var userId = httpContext.HttpContext.User.Identity.Name;

            string phoneNumber = value as string;

            if (string.IsNullOrEmpty(phoneNumber))
                return new ValidationResult("Phone Number is required ..!");

            if (!phoneNumber.All(char.IsDigit))
            {
                return new ValidationResult("Phone Number Must be Numbers Only ..!");
            }

            if (!phoneNumber.StartsWith("01"))
            {
                return new ValidationResult("Phone Number Must Starts with 01 ..!");
            }


            if (phoneNumber.Length != 11)
            {
                return new ValidationResult("Phone Number Length Must be 11 Digits ..!");
            }

            var secondDigit = phoneNumber[2];
            var validSecondDigits = new[] { '0', '1', '2', '5' };
            if (!validSecondDigits.Contains(secondDigit))
            {
                return new ValidationResult("Phone Number Must Follow Egyptian Communcation Providers ..!");

            }
            var isUsed = context.Users.Any(u => u.PhoneNumber == phoneNumber && u.UserName != userId);
            if (isUsed)
                return new ValidationResult("Phone Number Is Used Before ..!");

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-ParentPhone", "Phone Number must be a 11-digit number and not used by another user.");
        }
    }
}
