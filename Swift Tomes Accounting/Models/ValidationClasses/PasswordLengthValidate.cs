using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ValidationClasses
{
    public class PasswordLengthValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string temp = value.ToString();
            if (temp.Length < 8)
            {
                return new ValidationResult("Password must be 8 characters long.");
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
