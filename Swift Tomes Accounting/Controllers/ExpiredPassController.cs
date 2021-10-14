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
            var user_list = _db.ApplicationUser.ToList();
            List<ApplicationUser> expiredpass_list = new List<ApplicationUser>();

            foreach(var user in user_list)
            {
                if (user.PasswordDate.AddDays(30) < DateTime.Now)
                {
                    var roles = _db.Roles.ToList();
                    var userRoles = _db.UserRoles.ToList();
                    var role = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                    expiredpass_list.Add(user);
                }
            }

            return View(expiredpass_list);
        }
    }
}
