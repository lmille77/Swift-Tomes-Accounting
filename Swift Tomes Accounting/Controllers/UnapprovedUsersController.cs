using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var all_users = _db.ApplicationUser.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            List<ApplicationUser> unapproved_users = new List<ApplicationUser>();
            foreach(var user in all_users)
            {
                if(user.isApproved == false)
                {
                    var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;                    
                    unapproved_users.Add(user);
                }
            }
            return View(unapproved_users);
        }


        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var objFromdb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            var roles = _db.Roles.ToList();
            var userRoles = _db.UserRoles.ToList();
            var role = userRoles.FirstOrDefault(u => u.UserId == objFromdb.Id);
            objFromdb.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
            if (objFromdb == null)
            {
                return NotFound();
            }
            EventUser user_event = new EventUser
            {
                BeforeFname = objFromdb.FirstName,
                BeforeisActive = false,
                BeforeLname = objFromdb.LastName,
                BeforeuserName = objFromdb.CustomUsername,
                BeforeDOB = objFromdb.DOB,
                BeforeRole = objFromdb.Role,
                BeforeAddress = objFromdb.Address + " " + objFromdb.City + ", " + objFromdb.State + " " + objFromdb.ZipCode,
                AfterFname = "None",
                AfterisActive = false,
                AfterLname = "None",
                AfteruserName = "None",
                AfterDOB = "None",
                AfterRole = "None",
                AfterAddress = "None",
                eventTime = DateTime.Now,
                eventType = "Denied User",
                eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
            };
            _db.EventUser.Add(user_event);
            _db.ApplicationUser.Remove(objFromdb);
            _db.SaveChanges();
            TempData[SD.Success] = "User rejected succesfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Approve(string userId)
        {
            var objFromdb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromdb.Id);
            var roleList = _roleManager.Roles.ToList();
            var unapprovedID = "";
            var roles = _db.Roles.ToList();
            var userRoles = _db.UserRoles.ToList();
            objFromdb.Role = roles.FirstOrDefault(u => u.Id == userRole.RoleId).Name;
            
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
                    eventType = "Approved User",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventUser.Add(user_event);
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
                TempData[SD.Error] = "Assign a role before approving a user.";
            }


            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AssignRole(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == user.Id);
                var roles = _db.Roles.ToList();
                var userRoles = _db.UserRoles.ToList();
                var role = userRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                objFromDb.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;

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
                EventUser user_event = new EventUser
                {
                    BeforeFname = objFromDb.FirstName,
                    BeforeisActive = false,
                    BeforeLname = objFromDb.LastName,
                    BeforeuserName = objFromDb.CustomUsername,
                    BeforeDOB = objFromDb.DOB,
                    BeforeRole = objFromDb.Role,
                    BeforeAddress = objFromDb.Address + " " + objFromDb.City + ", " + objFromDb.State + " " + objFromDb.ZipCode,
                    AfterFname = user.FirstName,
                    AfterisActive = false,
                    AfterLname = user.LastName,
                    AfteruserName = objFromDb.CustomUsername,
                    AfterDOB = user.DOB,
                    AfterRole = user.Role,
                    AfterAddress = user.Address + " " + user.City + ", " + user.State + " " + user.ZipCode,
                    eventTime = DateTime.Now,
                    eventType = "Assigned Role",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventUser.Add(user_event);

                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;
                objFromDb.DOB = user.DOB;
                objFromDb.Address = user.Address;
                objFromDb.ZipCode = user.ZipCode;
                objFromDb.State = user.State;
                objFromDb.City = user.City;
                objFromDb.Role = user.Role;
                if(user.Role == null)
                {
                    user.Role = "Unapproved";
                }
                //add new role
                await _userManager.AddToRoleAsync(objFromDb, user.Role);

                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
    }
}
