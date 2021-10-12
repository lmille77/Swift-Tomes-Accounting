using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Controllers
{
    public class ExpiredPassController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public ExpiredPassController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            var expPassList = _db.ApplicationUser.ToList();

            foreach (var user in expPassList) // We don't need to show the hased password, only the users who have expired passwords
            {
                //this will find if there are any roles in the userRole table
                var expPass1 = user.LastPass1;
                var expPass2 = user.LastPass2;
                
            }
            //_db.Users.FirstOrDefault(u => u.LastPass1 != null);


            return View(expPassList);
        }
    }
}
