using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ValidationClasses
{
    public class LetterValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string temp = value.ToString();
            char[] temp_array = temp.ToCharArray();
            for (int i = 0; i < temp_array.Length; i++)
            {
                if (Char.IsLetter(temp_array[i]))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Password must contain a letter.");
        }
    }
}
