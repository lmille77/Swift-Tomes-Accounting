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

                if (item.AccountName.ToLower().Equals(account.AccountName.ToLower()))
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

            if (account.ChartOfAccounts && !account.Active)
            {
                ModelState.AddModelError("","Cannot add an Inactive account to Chart of Accounts.");
                return View(account);
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
            EventAccount new_account = new EventAccount
            {  
                BeforeAccountName = "None",
                BeforeAccountNumber = -1,
                BeforeDescription = "None",
                BeforeNormSide = "None",
                BeforeCategory = "None",
                BeforeOrder = -1,
                BeforeisActive = false,
                BeforeisContra = false,
                BeforeSubCategory = "None",
                BeforeInitial = -1,
                BeforeDebit = -1,
                BeforeCredit = -1,
                BeforeBalance = -1,
                BeforeStatement = "None",
                BeforeComments = "None",
                AfterAccountName = account.AccountName,
                AfterAccountNumber = account.AccountNumber,
                AfterDescription = account.Description,
                AfterNormSide = account.NormSide,
                AfterCategory = account.Category,
                AfterOrder = account.Order,
                AfterisActive = account.Active,
                AfterisContra = account.Contra,
                AfterSubCategory = account.SubCategory,
                AfterInitial = account.Initial,
                AfterDebit = account.Debit,
                AfterCredit = account.Credit,
                AfterBalance = account.Balance,
                AfterStatement = account.Statement,
                AfterComments = account.Comments,
                eventTime = DateTime.Now,
                eventType = "Created Account",
                eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
            };
            _db.EventAccount.Add(new_account);


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
        public IActionResult ChartOfAccounts(DateTime dateSearch1,
            DateTime dateSearch2, float balanceSearch1, float balanceSearch2)
        {
            var sortList = SearchResult(dateSearch1, dateSearch2, balanceSearch1, balanceSearch2);

            return View(sortList);

        }

        public IEnumerable<AccountDB> SearchResult(DateTime date1, DateTime date2, float balance1, float balance2)
        {
            var list = _db.Account.ToList();

            List<AccountDB> resultList = new List<AccountDB>();

            if (date1.ToString() != "1/1/0001 12:00:00 AM")
            {
                if (date2.ToString() == "1/1/0001 12:00:00 AM")
                {
                    date2 = DateTime.Now;
                }
                foreach (var item in list)
                {
                    if (date1 <= item.CreatedOn && item.CreatedOn <= date2)
                    {
                        resultList.Add(item);
                    }
                }
            }

            if ((balance1 > 0) && (balance2 == 0))
            {
                foreach(var item in list)
                {
                    if (balance1 <= item.Balance)
                    {
                        resultList.Add(item);
                    }
                }
            }
            if ((balance1 == 0) && (balance2 > 0))
            {
                foreach (var item in list)
                {
                    if (item.Balance <= balance2)
                    {
                        resultList.Add(item);
                    }
                }
            }
            if ((balance1 > 0) && (balance2 > 0))
            {
                foreach (var item in list)
                {
                    if ((balance1 <= item.Balance) && (item.Balance <= balance2))
                    {
                        resultList.Add(item);
                    }
                }
            }
                return resultList;
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
                if (obj.ChartOfAccounts && !objFromDb.Active)
                {
                    ModelState.AddModelError("", "Cannot add an Inactive account to Chart of Accounts.");
                    return View(obj);
                }

                EventAccount new_account = new EventAccount
                {
                    BeforeAccountName = objFromDb.AccountName,
                    BeforeAccountNumber = objFromDb.AccountNumber,
                    BeforeDescription = objFromDb.Description,
                    BeforeNormSide = objFromDb.NormSide,
                    BeforeCategory = objFromDb.Category,
                    BeforeOrder = objFromDb.Order,
                    BeforeisActive = objFromDb.Active,
                    BeforeisContra = objFromDb.Contra,
                    BeforeSubCategory = objFromDb.SubCategory,
                    BeforeInitial = objFromDb.Initial,
                    BeforeDebit = objFromDb.Debit,
                    BeforeCredit = objFromDb.Credit,
                    BeforeBalance = objFromDb.Balance,
                    BeforeStatement = objFromDb.Statement,
                    BeforeComments = objFromDb.Comments,
                    AfterAccountName = obj.AccountName,
                    AfterAccountNumber = obj.AccountNumber,
                    AfterDescription = obj.Description,
                    AfterNormSide = obj.NormSide,
                    AfterCategory = obj.Category,
                    AfterOrder = obj.Order,
                    AfterisActive = objFromDb.Active,
                    AfterisContra = objFromDb.Contra,
                    AfterSubCategory = obj.SubCategory,
                    AfterInitial = objFromDb.Initial,
                    AfterDebit = objFromDb.Debit,
                    AfterCredit = objFromDb.Credit,
                    AfterBalance = objFromDb.Balance,
                    AfterStatement = obj.Statement,
                    AfterComments = obj.Comments,
                    eventTime = DateTime.Now,
                    eventType = "Edited Account",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventAccount.Add(new_account);


                objFromDb.AccountNumber = obj.AccountNumber;
                objFromDb.AccountName = obj.AccountName;
                objFromDb.Description = obj.Description;
                objFromDb.NormSide = obj.NormSide; 
                objFromDb.Category = obj.Category;
                objFromDb.SubCategory = obj.SubCategory;
                objFromDb.Order = obj.Order;
                objFromDb.Statement = obj.Statement;
                objFromDb.Comments = obj.Comments;
                objFromDb.ChartOfAccounts = obj.ChartOfAccounts;
                
               
                _db.SaveChanges();
                TempData[SD.Success] = "Account has been edited successfully.";
                return RedirectToAction("ChartofAccounts", "Money");


            }

            return View(obj);

        }



        [HttpPost]
        public IActionResult Activate(double id)
        {
            var objFromDb = _db.Account.FirstOrDefault(u => u.AccountNumber == id);
           
            
            if (objFromDb == null)
            {
                return NotFound();
            }

            if (!objFromDb.Balance.Equals(0))
            {
                TempData[SD.Error] = "Account Balance must be $0.00 to Deactivate.";
                return RedirectToAction("ChartofAccounts", "Money");
            }

            if(objFromDb.Active == true)
            {
                EventAccount new_account = new EventAccount
                {
                    BeforeAccountName = objFromDb.AccountName,
                    BeforeAccountNumber = objFromDb.AccountNumber,
                    BeforeDescription = objFromDb.Description,
                    BeforeNormSide = objFromDb.NormSide,
                    BeforeCategory = objFromDb.Category,
                    BeforeOrder = objFromDb.Order,
                    BeforeisActive = objFromDb.Active,
                    BeforeisContra = objFromDb.Contra,
                    BeforeSubCategory = objFromDb.SubCategory,
                    BeforeInitial = objFromDb.Initial,
                    BeforeDebit = objFromDb.Debit,
                    BeforeCredit = objFromDb.Credit,
                    BeforeBalance = objFromDb.Balance,
                    BeforeStatement = objFromDb.Statement,
                    BeforeComments = objFromDb.Comments,
                    AfterAccountName = objFromDb.AccountName,
                    AfterAccountNumber = objFromDb.AccountNumber,
                    AfterDescription = objFromDb.Description,
                    AfterNormSide = objFromDb.NormSide,
                    AfterCategory = objFromDb.Category,
                    AfterOrder = objFromDb.Order,
                    AfterisActive = false,
                    AfterisContra = objFromDb.Contra,
                    AfterSubCategory = objFromDb.SubCategory,
                    AfterInitial = objFromDb.Initial,
                    AfterDebit = objFromDb.Debit,
                    AfterCredit = objFromDb.Credit,
                    AfterBalance = objFromDb.Balance,
                    AfterStatement = objFromDb.Statement,
                    AfterComments = objFromDb.Comments,
                    eventTime = DateTime.Now,
                    eventType = "Deactivated Account",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventAccount.Add(new_account);
                objFromDb.ChartOfAccounts = false;
                objFromDb.Active = false;
                TempData[SD.Success] = "Account deactivated successfully.";
            }

            else
            {
                EventAccount new_account = new EventAccount
                {
                    BeforeAccountName = objFromDb.AccountName,
                    BeforeAccountNumber = objFromDb.AccountNumber,
                    BeforeDescription = objFromDb.Description,
                    BeforeNormSide = objFromDb.NormSide,
                    BeforeCategory = objFromDb.Category,
                    BeforeOrder = objFromDb.Order,
                    BeforeisActive = objFromDb.Active,
                    BeforeisContra = objFromDb.Contra,
                    BeforeSubCategory = objFromDb.SubCategory,
                    BeforeInitial = objFromDb.Initial,
                    BeforeDebit = objFromDb.Debit,
                    BeforeCredit = objFromDb.Credit,
                    BeforeBalance = objFromDb.Balance,
                    BeforeStatement = objFromDb.Statement,
                    BeforeComments = objFromDb.Comments,
                    AfterAccountName = objFromDb.AccountName,
                    AfterAccountNumber = objFromDb.AccountNumber,
                    AfterDescription = objFromDb.Description,
                    AfterNormSide = objFromDb.NormSide,
                    AfterCategory = objFromDb.Category,
                    AfterOrder = objFromDb.Order,
                    AfterisActive = true,
                    AfterisContra = objFromDb.Contra,
                    AfterSubCategory = objFromDb.SubCategory,
                    AfterInitial = objFromDb.Initial,
                    AfterDebit = objFromDb.Debit,
                    AfterCredit = objFromDb.Credit,
                    AfterBalance = objFromDb.Balance,
                    AfterStatement = objFromDb.Statement,
                    AfterComments = objFromDb.Comments,
                    eventTime = DateTime.Now,
                    eventType = "Activated Account",
                    eventPerformedBy = _userManager.GetUserAsync(User).Result.FirstName + " " + _userManager.GetUserAsync(User).Result.LastName,
                };
                _db.EventAccount.Add(new_account);
                objFromDb.Active = true;
                TempData[SD.Success] = "Account activated successfully.";
            }           
           
            _db.SaveChanges();
            return RedirectToAction("ChartofAccounts", "Money");
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
        public IActionResult AccountLedger(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var objFromDb = _db.Account.FirstOrDefault(u => u.AccountName == id);
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
                return RedirectToAction("ChartofAccounts", "Money");
            }
            return View(objFromdb);
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var accountlist = _db.Account.ToList();
            return Json(new { data = accountlist });
        }

        
    }
}
