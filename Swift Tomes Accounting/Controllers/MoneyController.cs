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
            var errorList = _db.ErrorTable.ToList();
            foreach (var item in accountlist)
            {
                if (item.AccountNumber.Equals(account.AccountNumber))
                {
                    ModelState.AddModelError("", errorList[9].Message);
                    return View(account);
                }

                if (item.AccountName.ToLower().Equals(account.AccountName.ToLower()))
                {
                    ModelState.AddModelError("", errorList[1].Message);
                    return View(account);
                }

                if (item.Order.Equals(account.Order))
                {
                    ModelState.AddModelError("", errorList[10].Message);
                    return View(account);
                }
            }

            if (account.ChartOfAccounts && !account.Active)
            {
                ModelState.AddModelError("",errorList[11].Message);
                return View(account);
            }

            if (account.Order <= 0)
            {
                ModelState.AddModelError("", errorList[12].Message);
                return View(account);
            }

            if (account.Initial < 0)
            {
                ModelState.AddModelError("", errorList[13].Message);
                return View(account);
            }
            
            var temp = account.AccountNumber.ToString();

            string zero = null;

            if (account.AccountNumber <= 9)
            {
                zero = "0";
            }


            if (account.Category == "Asset")
            {
                temp = "1" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Expenses")
            {
                temp = "2" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Liability")
            {
                temp = "3" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Equity")
            {
                temp = "4" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Revenue")
            {
                temp = "5" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }

            account.CreatedOn = DateTime.Now;
            account.Balance = account.Initial;

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

          

            _db.Account.Add(account);
            await _db.SaveChangesAsync();
            return RedirectToAction("Accounts", "Money");
            
        }
        [HttpGet]
        public IActionResult Accounts()
        {
            var sortList = _db.Account.ToList();
            return View(sortList);

        }
        public IActionResult PostRef(int? id)
        {
            var all_accounts = _db.Journal_Accounts.ToList();
            List<Journal_Accounts> selected_accounts = new List<Journal_Accounts>();
            var jList = _db.Journalizes.ToList();

            foreach (var s in all_accounts)
            {
                if (s.JournalId == id)
                {
                    selected_accounts.Add(s);
                }
            }

            foreach (var s in selected_accounts)
            {
                foreach (var j in jList)
                {
                    if (s.JournalId == j.JournalId && j.isApproved == true)
                    {
                        s.IsApproved = true;
                    }
                }
            }

            return View(selected_accounts);
        }
        [HttpPost]
        public IActionResult Accounts(DateTime dateSearch1,
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
                var errorList = _db.ErrorTable.ToList();

                foreach (var item in accountlist)
                {

                    if ((item.AccountName.Equals(obj.AccountName)) && (!item.AccountName.Equals(objFromDb.AccountName)))
                    {
                        ModelState.AddModelError("", errorList[1].Message);
                        return View(obj);
                    }

                    if ((item.Order.Equals(obj.Order)) && (!item.Order.Equals(objFromDb.Order)))
                    {
                        ModelState.AddModelError("", errorList[10].Message);
                        return View(obj);
                    }

                }
                if (obj.ChartOfAccounts && !objFromDb.Active)
                {
                    ModelState.AddModelError("", errorList[11].Message);
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
                return RedirectToAction("Accounts", "Money");


            }

            return View(obj);

        }



        [HttpPost]
        public IActionResult Activate(double id)
        {
            var objFromDb = _db.Account.FirstOrDefault(u => u.AccountNumber == id);
            var errorList = _db.ErrorTable.ToList();
            
            if (objFromDb == null)
            {
                return NotFound();
            }

            if (!objFromDb.Balance.Equals(0))
            {
                TempData[SD.Error] = errorList[14].Message;
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
            var accountmatch = _db.Account.FirstOrDefault(u => u.AccountNumber == id);
            var ja = _db.Journal_Accounts.ToList();
            AccountLedger account_ledger = new AccountLedger
            {
                account = accountmatch,
                journal_accounts = ja
            };
            return View(account_ledger);

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

        public IActionResult AcctEventLog(int? id)
        {
            
            var all_acctevents = _db.EventAccount.ToList();
            List<EventAccount> select_account = new List<EventAccount>();
            foreach(var acct in all_acctevents)
            {
                if(acct.AfterAccountNumber == id)
                {
                    select_account.Add(acct);
                }
            }
            EventModel eventlist = new EventModel
            {
                EventAccount = select_account
            };
            return View(eventlist);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var accountlist = _db.Account.ToList();
            return Json(new { data = accountlist });
        }

        [HttpGet]
        public IActionResult LinkedName(string name)
        {
            if (name == null)
            {
                return NotFound();
            }
            var objFromDb = _db.Account.FirstOrDefault(u => u.AccountName == name);
            if (objFromDb == null)
            {
                return NotFound();
            }
            return View("AccountLedger",objFromDb);
        }



        public IActionResult JournalIndex()
        {
            var sortList = _db.Journal_Accounts.ToList();
            return View(sortList);

        }


    }
}
