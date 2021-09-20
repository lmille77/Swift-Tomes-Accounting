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
    public class UnapprovedUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        public UnapprovedUsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var all_users = await _userManager.GetUsersInRoleAsync("Unapproved");
            return View(all_users);
        }


        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            _db.ApplicationUser.Remove(objFromDb);
            _db.SaveChanges();
            TempData[SD.Success] = "User rejected succesfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Approve(string userId)
        {
            var objFromdb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromdb == null)
            {
                return NotFound();
            }

            if(objFromdb.isApproved == false)
            {
                //sends an email to admin requesting approval for new user
                //var email = _userManager.GetEmailAsync;
                //var subject = "Add new user";
                //var body = "<a href='https://localhost:44316/Account/Login'>Click to Add User </a>";
                //var mailHelper = new MailHelper(_configuration);
                //mailHelper.Send(_configuration["Gmail:Username"], admin_email, subject, body);

                objFromdb.isApproved = true;
                TempData[SD.Success] = "User approved successfully.";
            }

        
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
