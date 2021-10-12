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
                objFromDb.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
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
                await _userManager.AddToRoleAsync(objFromDb, user.RoleId);
                EventUser user_event = new EventUser
                {
                    BeforeFname = objFromDb.FirstName,
                    BeforeisActive = true,
                    BeforeLname = objFromDb.LastName,
                    BeforeuserName = objFromDb.CustomUsername,
                    BeforeDOB = objFromDb.DOB,
                    BeforeRole = objFromDb.Role,
                    BeforeAddress = objFromDb.Address + " " + objFromDb.City + ", " + objFromDb.State + " " + objFromDb.ZipCode,
                    AfterFname = user.FirstName,
                    AfterisActive = true,
                    AfterLname = user.LastName,
                    AfteruserName = objFromDb.CustomUsername,
                    AfterDOB = user.DOB,
                    AfterRole = user.RoleId,
                    AfterAddress = user.Address + " " + user.City + ", " + user.State + " " + user.ZipCode,
                    eventTime = DateTime.Now,
                    eventType = "Edited User ",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;
                objFromDb.DOB = user.DOB;
                objFromDb.Address = user.Address;
                objFromDb.ZipCode = user.ZipCode;
                objFromDb.State = user.State;
                objFromDb.City = user.City;
                
                
                _db.EventUser.Add(user_event);
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
                EventUser user_event = new EventUser
                {
                    BeforeFname = objFromdb.FirstName,
                    BeforeisActive = false,
                    BeforeLname = objFromdb.LastName,
                    BeforeuserName = objFromdb.CustomUsername,
                    BeforeDOB = objFromdb.DOB,
                    BeforeRole = objFromdb.Role,
                    BeforeAddress = objFromdb.Address + " " + objFromdb.City + ", " + objFromdb.State + " " + objFromdb.ZipCode,
                    AfterFname = objFromdb.FirstName,
                    AfterisActive = true,
                    AfterLname = objFromdb.LastName,
                    AfteruserName = objFromdb.CustomUsername,
                    AfterDOB = objFromdb.DOB,
                    AfterRole = objFromdb.Role,
                    AfterAddress = objFromdb.Address + " " + objFromdb.City + ", " + objFromdb.State + " " + objFromdb.ZipCode,
                    eventTime = DateTime.Now,
                    eventType = "Unlocked User",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventUser.Add(user_event);
                //this mean user is locked and will remain lcoked until lockoutend time
                //clickng will unlock user
                objFromdb.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "User unlocked successfully.";
            }
            else
            {
                EventUser user_event = new EventUser
                {
                    BeforeFname = objFromdb.FirstName,
                    BeforeisActive = true,
                    BeforeLname = objFromdb.LastName,
                    BeforeuserName = objFromdb.CustomUsername,
                    BeforeDOB = objFromdb.DOB,
                    BeforeRole = objFromdb.Role,
                    BeforeAddress = objFromdb.Address + " " + objFromdb.City + ", " + objFromdb.State + " " + objFromdb.ZipCode,
                    AfterFname = objFromdb.FirstName,
                    AfterisActive = false,
                    AfterLname = objFromdb.LastName,
                    AfteruserName = objFromdb.CustomUsername,
                    AfterDOB = objFromdb.DOB,
                    AfterRole = objFromdb.Role,
                    AfterAddress = objFromdb.Address + " " + objFromdb.City + ", " + objFromdb.State + " " + objFromdb.ZipCode,
                    eventTime = DateTime.Now,
                    eventType = "Locked User",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventUser.Add(user_event);
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

        
        

    }
}

