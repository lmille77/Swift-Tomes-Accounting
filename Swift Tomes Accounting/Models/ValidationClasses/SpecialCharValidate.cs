using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ValidationClasses
{
    public class SpecialCharValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string temp = value.ToString();
            Regex rgx = new Regex("[^A-Za-z0-9]");

            if (rgx.IsMatch(temp))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Password must contain a special character.");
        }
    }
}
