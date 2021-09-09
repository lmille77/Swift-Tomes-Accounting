using Swift_Tomes_Accounting.Models.ValidationClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class RegisterViewModel
    {        

        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; } 
        
        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName {get;set;}     
            
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [PasswordFormatValidate]
        [FirstCharCapitalValidate]
        [FirstCharValidate]
        [PasswordLengthValidate]
        [NumberValidate]
        [LetterValidate]
        [SpecialCharValidate]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }
        
    }
}
