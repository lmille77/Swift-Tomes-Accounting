using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class FirstCharCapitalValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string temp = value.ToString();
            char[] password = temp.ToCharArray();
            if (!Char.IsUpper(password[0]))
            {
                return new ValidationResult("First character of the password must be capitalized.");
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
