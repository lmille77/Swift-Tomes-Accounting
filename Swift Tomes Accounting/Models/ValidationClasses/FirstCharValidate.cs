using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class FirstCharValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string temp = value.ToString();
            char[] password = temp.ToCharArray();
            if (!Char.IsLetter(password[0]))
            {
                return new ValidationResult("First character of password must be a letter.");
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
