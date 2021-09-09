using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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

    }
}
