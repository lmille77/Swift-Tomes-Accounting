using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Helpers
{
    public class RoleHelper
    {
        
        public static List<SelectListItem> GetRolesForDropDown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{Value="Unapproved", Text="Unapproved"},
                new SelectListItem{Value="Accountant", Text="Accountant"},
                new SelectListItem{Value="Manager", Text="Manager"},
                new SelectListItem{Value="Admin", Text="Admin"}
            };
        }
    }
}
