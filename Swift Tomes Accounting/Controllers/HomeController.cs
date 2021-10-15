using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swift_Tomes_Accounting.Models;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                return RedirectToAction("Index", "Manager");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Accountant"))
            {
                return RedirectToAction("Index", "Accountant");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Unapproved"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Help()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
