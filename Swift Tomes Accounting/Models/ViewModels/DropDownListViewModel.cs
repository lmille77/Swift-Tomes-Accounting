using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class DropDownListViewModel
    {
        public string SelectedValue { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }
}
