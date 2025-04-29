using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVCProject.Validations
{
    public class ParentNumberValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string parentPhone = value as string;

            if (string.IsNullOrEmpty(parentPhone))
                return ValidationResult.Success;

            if (!parentPhone.All(char.IsDigit))
            {
                return new ValidationResult("Parent Number Must be Numbers Only ..!");
            }

            if (!parentPhone.StartsWith("01"))
            {
                return new ValidationResult("Parent Number Must Starts with 01 ..!");
            }


            if (parentPhone.Length != 11)
            {
                return new ValidationResult("Parent Number Length Must be 11 Digits ..!");
            }

            var secondDigit = parentPhone[2];
            var validSecondDigits = new[] { '0', '1', '2', '5' };
            if (!validSecondDigits.Contains(secondDigit))
            {
                return new ValidationResult("Parent Number Must Follow Egyptian Communcation Providers ..!");

            }

             return ValidationResult.Success;
        }
        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-phonenumber", "Phone Number must be a 11-digit number and not used by another user.");
        }
    }
}
