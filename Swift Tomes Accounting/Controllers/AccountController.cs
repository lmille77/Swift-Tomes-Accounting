using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.Helpers;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;




namespace Swift_Tomes_Accounting.Controllers
{
    
    public class AccountController : Controller
    {
        //variables used to host email service
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;        

        
        private readonly ApplicationDbContext _db;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
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

        

        [HttpGet]
        public async Task<IActionResult> Login()
        {         

            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Manager"));
                await _roleManager.CreateAsync(new IdentityRole("Accountant"));
                await _roleManager.CreateAsync(new IdentityRole("Unapproved"));
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "miller4277@gmail.com",
                    CustomUsername = "Admin1",
                    FirstName = "Default",
                    LastName = "Admin",
                    Email = "miller4277@gmail.com",
                    isApproved = true,
                    Role = "Admin",
                    PasswordDate = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, "Admin123!");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
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
                    return RedirectToAction("Index", "Home");
                }
            }
                return View();
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel obj)
        {
            

            if (ModelState.IsValid)
            {
                //checks database to see if user and password are correct
                var result = await _signInManager.PasswordSignInAsync(obj.Email, obj.Password, false, lockoutOnFailure: true);
                var user = await _userManager.FindByNameAsync(obj.Email);
                int num = 0;
                if(user != null)
                {
                    num = 3 - user.AccessFailedCount;
                }
                
                //error list
                var errorList = _db.ErrorTable.ToList();

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                if (result.Succeeded)
                {
                    //checks to see if a user has been approved by an admin and redirects accordingly
                    var curr_user = await _userManager.FindByNameAsync(obj.Email);
                    var admin_role_list = await _userManager.GetUsersInRoleAsync("Admin");
                    var manager_role_list = await _userManager.GetUsersInRoleAsync("Manager");
                    var accountant_role_list = await _userManager.GetUsersInRoleAsync("Accountant");

                    //find DateTime of when password was created
                    var lastpassdate = curr_user.PasswordDate;
                    
                    
                    if (lastpassdate.AddDays(30) < DateTime.Now)
                    {
                        TempData[SD.Error] = errorList[0].Message;

                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var callbackurl = Url.Action("ConfirmResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                        //sends an email to admin requesting approval for new user
                        var subject = "Your password has expired!";


                        var body = "Your password has expired.\n" +
                            "You will not be able to login unless you reset your password.\n" +
                            "Please reset your password by clicking <a href=\"" + callbackurl + "\"> here</a>.";

                        var mailHelper = new MailHelper(_configuration);
                        mailHelper.Send(_configuration["Gmail:Username"], user.Email, subject, body);

                        await _signInManager.SignOutAsync();
                        return View();
                    }
                    if (lastpassdate.AddDays(27) < DateTime.Now)
                    {
                        TempData[SD.Error] = errorList[2].Message;
                    }
                    if(lastpassdate.AddDays(30) > DateTime.Now)
                    {
                        if (curr_user.isApproved == true && admin_role_list.Contains(curr_user))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (curr_user.isApproved == true && manager_role_list.Contains(curr_user))
                        {
                            return RedirectToAction("Index", "Manager");
                        }
                        else if (curr_user.isApproved == true && accountant_role_list.Contains(curr_user))
                        {
                            return RedirectToAction("Index", "Accountant");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }  
                }
                else
                {
                    ModelState.AddModelError("", errorList[3].Message);
                    if(user != null)
                    {
                        TempData[SD.Error] = "Attempts remaining: " + num;
                    }
                    
                }
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Register()
        {
            //if the user roles are not already stored in the database, then they are added            
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
                return RedirectToAction("Index", "Home");
            }

            return View();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel obj)
        {

            string _Firstname = obj.FirstName.ToLower();
            string _Lastname = obj.LastName.ToLower();
            var errorList = _db.ErrorTable.ToList();

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
                    isApproved = false,
                    DOB = obj.DOB,
                    Address = obj.Address,
                    PasswordDate = DateTime.Now,
                    ZipCode = obj.ZipCode,
                    State = obj.State,
                    City = obj.City,
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
                    AfterisActive = false,
                    AfterLname = obj.LastName,
                    AfteruserName = _Firstname[0] + _Lastname + DateTime.Now.ToString("yyMM"),
                    AfterDOB = obj.DOB,
                    AfterRole = "Unapproved",
                    AfterAddress = obj.Address + " " + obj.City + ", " + obj.State + " " + obj.ZipCode,
                    eventTime = DateTime.Now,
                    eventType = "Requested Access",
                    eventPerformedBy = obj.FirstName + " " + obj.LastName,
                };  
                
                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);

                //finds  all admin user
                var admin_users = await _userManager.GetUsersInRoleAsync("Admin");
                var admin_email = "";

                foreach (ApplicationUser admin_user in admin_users)
                {
                    if (admin_user.isApproved == true)
                    {
                        admin_email = admin_user.Email;
                        break;
                    }
                }

                if (result.Succeeded)
                {
                    
                    //sends an email to admin requesting approval for new user
                    var subject = "Add new user";
                    var body = "<a href='https://localhost:44316/Account/Login'>Click to Add User </a>";
                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], admin_email, subject, body);
                    
                    //adds user to database but without admin approval
                    await _userManager.AddToRoleAsync(user, "Unapproved");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _db.EventUser.Add(user_event);
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", errorList[4].Message);
                }
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logoff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        
        //Password Reset Action

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(Message obj)
        {
            var user = await _userManager.FindByEmailAsync(obj.ToEmail);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            //sends them to 
            var callbackurl = Url.Action("ConfirmResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            //"<a href='https://localhost:44316/Account/Login'>Click to Add User </a>"

            var toEmail = obj.ToEmail;
            var subject = "Password Reset Confirmation";
            var body = "Please reset your password by clicking <a href=\"" + callbackurl + "\"> here</a>.";
            var mailHelper = new MailHelper(_configuration);
            mailHelper.Send(_configuration["Gmail:Username"], toEmail, subject, body);
            
            TempData[SD.Success] = "Check email for password reset link.";
            return RedirectToAction("Index", "Admin");
         
        }
       

        
        //Password Reset Confirmation Action
        
        [HttpGet]
        public IActionResult ConfirmResetPassword(string userId, string code = null)
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
            PasswdReset newobj = new PasswdReset();
            newobj.Email = objFromDb.Email;
            return code == null ? View("Error") : View(newobj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmResetPassword(PasswdReset obj)
        {
            if (ModelState.IsValid)
            {
                var curr_user = await _userManager.FindByNameAsync(obj.Email);
                
                bool passcheck = false;

                var errorList = _db.ErrorTable.ToList();

                var result = await _userManager.CheckPasswordAsync(curr_user, obj.NewPass);

                PasswordVerificationResult passMatch = PasswordVerificationResult.Failed;
                PasswordVerificationResult passMatch2 = PasswordVerificationResult.Failed;

                if (curr_user.LastPass1 != null)
                {
                     passMatch = _userManager.PasswordHasher.VerifyHashedPassword(curr_user, curr_user.LastPass1, obj.NewPass);
                }

                if (curr_user.LastPass2 != null)
                {
                    passMatch2 = _userManager.PasswordHasher.VerifyHashedPassword(curr_user, curr_user.LastPass2, obj.NewPass);
                }

                if (result == true)
                {
                    ViewBag.ErrorMessage = errorList[5].Message;
                }

                else if (passMatch == PasswordVerificationResult.Success)
                 {
                        ViewBag.ErrorMessage = errorList[5].Message;
                 }
                
                else if (passMatch2 == PasswordVerificationResult.Success)
                    {
                        ViewBag.ErrorMessage = errorList[5].Message;
                    }
                
                else
                    passcheck = true;

                if (passcheck == true)
                {

                    curr_user.LastPass2 = curr_user.LastPass1;
                    curr_user.LastPass1 = curr_user.PasswordHash;
                    curr_user.PasswordDate = DateTime.Now;
                    await _userManager.UpdateAsync(curr_user);

                    await _userManager.RemovePasswordAsync(curr_user);
                    await _userManager.ResetPasswordAsync(curr_user, obj.Code, obj.NewPass);
                    await _userManager.AddPasswordAsync(curr_user, obj.NewPass);

                    TempData[SD.Success] = "Password was successfully reset.";
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(obj);
        }

       


        //[HttpPost]
        //public IActionResult AjaxMethod()
        //{
        //    List<Account> customers = (from customer in this.Context.Customers
        //                                select customer).ToList();
        //    return Json(JsonConvert.SerializeObject(customers));
        //}

    }
}
