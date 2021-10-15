using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Controllers
{
    public class Report : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
