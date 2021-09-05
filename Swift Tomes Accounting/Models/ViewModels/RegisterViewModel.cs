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
        public string _Username, _FirstName, _LastName;

        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { 
            get => _FirstName; 
            set => _FirstName = value.ToLower(); 
        }
        
        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName {
            get => _LastName;
            set => _LastName = value.ToLower();
        }
        
        public string Username {
            get => _Username;
            set 
            {
                _Username = _FirstName[0] + _LastName + DateTime.Now.Month.ToString("yyMM"); 
            }
        }
        
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [PasswordFormatValidate]
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
