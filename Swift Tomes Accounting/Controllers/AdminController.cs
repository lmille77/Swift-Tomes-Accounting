using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.Helpers;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.IO;
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

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                // var all_users = await _userManager.GetUsersInRoleAsync("Unapproved");

                var account = _db.Account.ToList();
                
                double current = 0;
                double casset = 0;
                double cliab = 0;

                double roa = 0;
                double neti = 0;
                double tasset = 0;
                double exp = 0;

                double roe = 0;
                double shareeq = 0;

                double margin = 0;
                double rev = 0;

                double turnover = 0;
                double sales = 0;

                double quick = 0;
                double inv = 0;

                foreach (var item in account)
                {
                    if (item.Category == "Asset" && item.SubCategory == "Current")
                    {
                        casset += item.Balance;
                    }
                    if (item.Category == "Liability" && item.SubCategory == "Current")
                    {
                        cliab += item.Balance;
                    }
                    if (item.AccountName == "Merchandise Inventory")
                    {
                        inv += item.Balance;
                    }
                    if (item.AccountName == "Sales")
                    {
                        sales += item.Balance;
                    }
                    if (item.AccountName == "Contributed Capital")
                    {
                        shareeq += item.Balance;
                    }
                    if (item.Category == "Revenue")
                    {
                        rev += item.Balance;
                    }
                    if (item.Category == "Expenses")
                    {
                        exp += item.Balance;
                    }
                    if (item.Category == "Asset")
                    {
                        tasset += item.Balance;
                    }

                }

                neti = rev - exp; 

                current = casset / cliab;
                quick = (casset - inv - inv) / cliab;              
                roa = neti / tasset;
                turnover = sales / (tasset / 2);
                roe = neti / shareeq;
                margin = (neti / rev) * 100;



                Ratio ratio = new Ratio()
                {
                    Current = current,
                    RoA = roa,
                    RoE = roe,
                    Margin = margin,
                    Turnover = turnover,
                    Quick = quick

                };

                return View(ratio);
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
        //Action for Sending Admin Messages

        [HttpGet]
        public IActionResult Send()
        {
            var userList = _db.ApplicationUser.ToList();
            List<SelectListItem> users = new List<SelectListItem>();
            foreach(var user in userList)
            {
                SelectListItem li = new SelectListItem
                {
                    Value = user.Email,
                    Text = user.LastName + ", " + user.FirstName + " <" + user.Email + ">"

                };
                users.Add(li);
                ViewBag.Users = users;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Send(Message obj)
        {
            var toEmail = obj.ToEmail;
            var subject = obj.Subject;
            var body = obj.Body;
            var mailHelper = new MailHelper(_configuration);
            mailHelper.Send(_configuration["Gmail:Username"], toEmail, subject, body);            
            return RedirectToAction("Index","Admin");
        }
        
        [HttpPost]
        public IActionResult GoBack()
        {
            return RedirectToAction("Index", "Admin");

        }


        [HttpGet]
        public IActionResult EventLog()
        {
            var userevents = _db.EventUser.ToList();
            var accountevents = _db.EventAccount.ToList();

            

            EventModel EventModel = new EventModel()
            {
                EventUser = userevents,
                EventAccount = accountevents
                
            };
            
            return View(EventModel);
        }

        [HttpGet]
        public IActionResult Journalize()
        {            
            Journalize journalize = new Journalize();
            journalize.AccountList = _db.Account.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = u.AccountName,
                Text = u.AccountName
            });

            journalize.Journal_Accounts.Add(new Journal_Accounts() { JAId = 1 });
            
            return View(journalize);
        }

        [HttpPost]
        public IActionResult Journalize(Journalize journal)
        {
            if (ModelState.IsValid)
            {
                
                _db.Journalizes.Add(journal);
                 _db.SaveChanges();
                TempData[SD.Success] = "Journal entry submitted";
                return RedirectToAction("Index", "Admin");
            }
            

            return View(journal);

        }

      


        public IActionResult Report()
        {

            return View();

        }
        public IActionResult ExpiredPass()
        {
            var user_list = _db.ApplicationUser.ToList();
            List<ApplicationUser> expiredpass_list = new List<ApplicationUser>();

            foreach (var user in user_list)
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
        public IActionResult AddJournalEntry()
        {
            return View();
        }
    }
}
