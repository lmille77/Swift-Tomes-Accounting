using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.Helpers;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Controllers
{
    public class AdminController : Controller
    {
        //variables used to host email service
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        private readonly ApplicationDbContext _db;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
         IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                var all_users = await _userManager.GetUsersInRoleAsync("Unapproved");
                return View(all_users);
            }            
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Accountant"))
            {
                return RedirectToAction("Index", "Accountant");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                return RedirectToAction("Index", "Manager");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Unapproved"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
        [HttpGet]
        public IActionResult Send()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Send(Message obj)
        {
            var toEmail = obj.ToEmail;
            var subject = obj.Subject;
            var body = obj.Body;
            var mailHelper = new MailHelper(configuration);
            mailHelper.Send(configuration["Gmail:Username"], toEmail, subject, body);            
            return RedirectToAction("Index","Admin");
        }

    }
}
