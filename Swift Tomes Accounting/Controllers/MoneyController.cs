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
    public class MoneyController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MoneyController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(AccountDB account)
        {
            account.UserName = _userManager.GetUserAsync(User).Result.CustomUsername;
            var accountlist = _db.Account.ToList();
            foreach (var item in accountlist)
            {
                if (item.AccountNumber.Equals(account.AccountNumber))
                {
                    ModelState.AddModelError("", "Account Number already in use.");
                    return View(account);
                }

                if (item.AccountName.Equals(account.AccountName))
                {
                    ModelState.AddModelError("", "Account Name already in use.");
                    return View(account);
                }

                if (item.Order.Equals(account.Order))
                {
                    ModelState.AddModelError("", "Liquidity Order already in use.");
                    return View(account);
                }
            }

            if (account.Order <= 0)
            {
                ModelState.AddModelError("", "Liquidity must be a positive number.");
                return View(account);
            }

            if (account.Initial < 0)
            {
                ModelState.AddModelError("", "Initial Balance cannot be negative.");
                return View(account);
            }

            account.CreatedOn = DateTime.Now;
            _db.Account.Add(account);
            await _db.SaveChangesAsync();
            return RedirectToAction("ChartofAccounts", "Money");
            
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
                if (item.ChartOfAccounts)
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


        [HttpGet]
        public IActionResult EditAccount(double? id)
        {

             if (id == null)
            {
                return NotFound();
            }

            var objFromDb = _db.Account.FirstOrDefault(u => u.AccountNumber == id);

            if(objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }




        [HttpPost]
        public IActionResult EditAccount(AccountDB obj)
        {

            if (ModelState.IsValid)
            {


                var accountlist = _db.Account.ToList();
                var objFromDb = _db.Account.FirstOrDefault(u => u.AccountNumber == obj.AccountNumber);


                foreach (var item in accountlist)
                {

                    if ((item.AccountName.Equals(obj.AccountName)) && (!item.AccountName.Equals(objFromDb.AccountName)))
                    {
                        ModelState.AddModelError("", "Account Name already in use.");
                        return View(obj);
                    }

                    if ((item.Order.Equals(obj.Order)) && (!item.Order.Equals(objFromDb.Order)))
                    {
                        ModelState.AddModelError("", "Liquidity Order already in use.");
                        return View(obj);
                    }

                }



                objFromDb.AccountNumber = obj.AccountNumber;
                objFromDb.AccountName = obj.AccountName;
                objFromDb.Description = obj.Description;
                objFromDb.NormSide = obj.NormSide; 
                objFromDb.Category = obj.Category;
                objFromDb.SubCategory = obj.SubCategory;
                objFromDb.Order = obj.Order;
                objFromDb.Statement = obj.Statement;
                objFromDb.Comments = obj.Comments;
                objFromDb.Active = true;
               
                _db.SaveChanges();
                TempData[SD.Success] = "Account has been edited successfully.";
                return RedirectToAction("ChartofAccounts", "Money");


            }

            return View(obj);

        }



        [HttpPost]
        public IActionResult Activate(double id)
        {
            var objFromdb = _db.Account.FirstOrDefault(u => u.AccountNumber == id);
           
            
            if (objFromdb == null)
            {
                return NotFound();
            }


            if(objFromdb.Active == true)
            {
                objFromdb.Active = false;
                TempData[SD.Success] = "Account deactivated successfully.";
            }

            else
            {
                objFromdb.Active = true;
                TempData[SD.Success] = "Account activated successfully.";
            }
            
           
            _db.SaveChanges();
            return RedirectToAction("ChartofAccounts", "Money");
        }

    }
}
