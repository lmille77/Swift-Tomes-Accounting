using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ValidationClasses
{
    public class DateFormatValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string temp = value.ToString();
            int digitCounter = 0;
            for(int i = 0; i < temp.Length; i++)
            {
                if(Char.IsDigit(temp[i]))
                {
                    digitCounter++;
                }
            }
            if (temp[2] != '/' || temp[5] != '/' || temp.Length != 10 || digitCounter != 8)
            {
                return new ValidationResult("Date must be in this format: mm/dd/yyyy");
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
