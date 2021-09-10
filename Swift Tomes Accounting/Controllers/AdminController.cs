using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swift_Tomes_Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Controllers
{
    public class AdminController : Controller
    {
        private IConfiguration configuration;
        private IWebHostEnvironment webHostEnvironment;

        public AdminController(IConfiguration _configuration, IWebHostEnvironment _webHostEnvironment)
        {
            configuration = _configuration;
            webHostEnvironment = _webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Send()
        {
            var body = " ";
            var subject = " ";
            var mailHelper = new MailHelper(configuration);
            mailHelper.Send(configuration["Gmail:Username"], " ", " ", " ");            
            return View();
        }

    }
}
