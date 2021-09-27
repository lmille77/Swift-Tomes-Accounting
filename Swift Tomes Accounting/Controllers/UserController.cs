using Microsoft.AspNetCore.Identity;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.Models.ViewModels;
using Swift_Tomes_Accounting.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swift_Tomes_Accounting;
using Swift_Tomes_Accounting.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace NewSwift.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        public UserController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            var userList = _db.ApplicationUser.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                //this will find if there are any roles in the userRole table
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }

            return View(userList);

        }


        public IActionResult Edit(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }


            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            //this will find if there are any roles assigned to the user
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if (role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }
            objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

            return View(objFromDb);
        }

        public IActionResult AssignRole(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }


            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            //this will find if there are any roles assigned to the user
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if (role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }
            objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();
                }
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                if (userRole != null)
                {
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                    //removing the old role
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);

                }

                //add new role
                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);

                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;
                objFromDb.DOB = user.DOB;
                objFromDb.Address = user.Address;
                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index));
            }


            user.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }

        [HttpPost]
        public IActionResult LockUnlock(string userId)
        {
            var objFromdb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromdb == null)
            {
                return NotFound();
            }

            if (objFromdb.LockoutEnd != null && objFromdb.LockoutEnd > DateTime.Now)
            {
                //this mean user is locked and will remain lcoked until lockoutend time
                //clickng will unlock user
                objFromdb.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "User unlocked successfully.";
            }
            else
            {
                //user is not locked and we want to lock the user
                objFromdb.LockoutEnd = DateTime.Now.AddYears(1000);
                TempData[SD.Success] = "User locked successfully.";
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
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
            TempData[SD.Success] = "User deleted succesfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string userId)
        {
            var objFromdb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromdb.Id);
            var roleList = _roleManager.Roles.ToList();
            var unapprovedID = "";
            foreach (var role in roleList)
            {
                if (role.Name == "Unapproved")
                {
                    unapprovedID = role.Id;
                }
            }
            if (objFromdb == null)
            {
                return NotFound();
            }

            if (objFromdb.isApproved == false && userRole.RoleId != unapprovedID)
            {
                //sends an email to admin requesting approval for new user
                var email = objFromdb.Email;
                var subject = "Accepted";
                var body = "<a href='https://localhost:44316/Account/Login'>Click here to sign in </a>";
                var mailHelper = new MailHelper(_configuration);
                mailHelper.Send(_configuration["Gmail:Username"], email, subject, body);
                objFromdb.isApproved = true;
                TempData[SD.Success] = "User approved successfully.";
            }
            else
            {

            }


            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();
                }
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                if (userRole != null)
                {
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                    //removing the old role
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);

                }

                //add new role
                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);                
                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

    }
}

