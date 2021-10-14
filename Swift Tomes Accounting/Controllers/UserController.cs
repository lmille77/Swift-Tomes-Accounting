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
        SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        public UserController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel obj)
        {
            string _Firstname = obj.FirstName.ToLower();
            string _Lastname = obj.LastName.ToLower();

            if (ModelState.IsValid)
            {
                //object created by user input
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = obj.Email,
                    CustomUsername = _Firstname[0] + _Lastname + DateTime.Now.ToString("yyMM"),
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    Email = obj.Email,
                    isApproved = true,
                };

                EventUser user_event = new EventUser
                {
                    BeforeFname = "None",
                    BeforeisActive = false,
                    BeforeLname = "None",
                    BeforeuserName = "None",
                    BeforeDOB = "None",
                    BeforeRole = "None",
                    BeforeAddress = "None",
                    AfterFname = obj.FirstName,
                    AfterisActive = true,
                    AfterLname = obj.LastName,
                    AfteruserName = _Firstname[0] + _Lastname + DateTime.Now.ToString("yyMM"),
                    AfterDOB = "None",
                    AfterRole = obj.Role,
                    AfterAddress = "None",
                    eventTime = DateTime.Now,
                    eventType = "Created User",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventUser.Add(user_event);

                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);
                

                if (result.Succeeded)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackurl = Url.Action("ConfirmResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    //sends an email to admin requesting approval for new user
                    var subject = "Your account has been created!";


                    var body = "Your account has been created.\n" +
                        "You will not be able to login unless you create a password.\n" +
                        "Please create your password by clicking <a href=\"" + callbackurl + "\"> here</a>.";

                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], user.Email, subject, body);

                    
                    await _userManager.AddToRoleAsync(user, obj.Role);

                    _db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "An account with the entered email already exists.");
                }

            }
            return View();
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

                //add new role
                await _userManager.AddToRoleAsync(objFromDb, user.Role);
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
                    AfterRole = user.Role,
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
            var roles = _db.Roles.ToList();
            var userRoles = _db.UserRoles.ToList();
            var role = userRoles.FirstOrDefault(u => u.UserId == objFromdb.Id);
            objFromdb.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
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

