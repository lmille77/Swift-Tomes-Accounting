using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        //columns added to the database
        public string FirstName { get; set; }
        public string LastName { get; set; }        
        public string CustomUsername { get; set; }
        public bool isApproved { get; set; }

        public string LastPass1 { get; set; }

        public string LastPass2 { get; set; }

        public DateTime PasswordDate { get; set; }

        public string Address { get; set; }

        public string DOB { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        //just used to display in the view, not stored in database
        [NotMapped]
        public string RoleId { get; set; }

        //just used to display in the view, not stored in database
        [NotMapped]
        public string Role { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> RoleList { get; set; }
        [NotMapped]
        public string eventPerformedBy { get; set; }


    }
}
