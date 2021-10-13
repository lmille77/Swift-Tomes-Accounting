using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Swift_Tomes_Accounting.Controllers
{
    public class ManagerController : Controller
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public ManagerController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
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

            if (_signInManager.IsSignedIn(User) && User.IsInRole("Manager"))
            {
                return View();
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Accountant"))
            {
                return RedirectToAction("Index", "Accountant");
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


        [HttpGet]
        public IActionResult ChartOfAccounts()
        {
            var sortList = _db.Account.ToList();
            return View(sortList);

        }

        [HttpPost]
        public IActionResult ChartOfAccounts(string searchString)
        {
            var sortList = SearchResult(searchString);
            ViewBag.result = searchString;
            return View(sortList);

        }

        public IEnumerable<AccountDB> SearchResult(string search)
        {
            var list = _db.Account.ToList();

            List<AccountDB> activeList = new List<AccountDB>();

            foreach (var item in list)
            {
                if ((item.ChartOfAccounts) && (item.Active))
                {
                    activeList.Add(item);
                }
            }

            if (!String.IsNullOrEmpty(search))
            {
                List<AccountDB> resultList = new List<AccountDB>();
                foreach (var item in activeList)
                {

                    if (item.AccountName.Contains(search))
                    {
                        resultList.Add(item);
                    }

                    else if (item.AccountNumber.ToString().Contains(search))
                    {
                        resultList.Add(item);
                    }

                }

                return resultList;
            }

            return activeList;
        }
        public IActionResult Report()
        {
           
            return View();

        }

        [HttpGet]

        public IActionResult AccountLedger(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var objFromDb = _db.Account.FirstOrDefault(u => u.AccountNumber == id);
            if (objFromDb == null)
            {
                return NotFound();
            }
            return View(objFromDb);

        }



        [HttpPost]
        public IActionResult AccountLedger(int id)
        {
            var objFromdb = _db.Account.FirstOrDefault(u => u.AccountNumber == id);


            if (objFromdb == null)
            {
                return NotFound();
            }


            if (objFromdb.Active == true)
            {
                _db.SaveChanges();
                return RedirectToAction("ChartofAccounts", "Manager");
            }
            return View(objFromdb);
        }



    }
}
