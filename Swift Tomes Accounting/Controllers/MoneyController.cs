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
                account.Statement = "Balance Sheet";
                account.NormSide = "Left";
                if (account.Contra)
                {
                    account.NormSide = "Right";
                }
                temp = "1" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Expenses")
            {
                account.Statement = "Income Statement";
                account.NormSide = "Left";
                if (account.Contra)
                {
                    account.NormSide = "Right";
                }
                temp = "2" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Liability")
            {
                account.Statement = "Balance Sheet";
                account.NormSide = "Right";
                if (account.Contra)
                {
                    account.NormSide = "Left";
                }
                temp = "3" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Equity")
            {
                account.Statement = "Balance Sheet";
                account.NormSide = "Right";
                if (account.Contra)
                {
                    account.NormSide = "Left";
                }
                temp = "4" + zero + temp;
                account.AccountNumber = double.Parse(temp);
            }
            else if (account.Category == "Revenue")
            {
                account.Statement = "Income Statement";
                account.NormSide = "Right";
                if (account.Contra)
                {
                    account.NormSide = "Left";
                }
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
                        s.Description = j.Description;
                        s.Type = j.Type;
                        s.IsApproved = true;
                        s.docUrl = j.docUrl;
                    }
                }
            }

            return View(selected_accounts);
        }

        [HttpGet]
        public IActionResult Accounts()
        {
            var sortList = _db.Account.ToList();
            return View(sortList);

        }
        [HttpPost]
        public IActionResult Accounts(DateTime dateSearch1,
            DateTime dateSearch2, float balanceSearch1, float balanceSearch2)
        {
            var sortList = SearchResult(dateSearch1, dateSearch2, balanceSearch1, balanceSearch2);

            return View(sortList);

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


                if (obj.Category == "Asset")
                {
                    obj.Statement = "Balance Sheet";
                    obj.NormSide = "Left";
                    if (obj.Contra)
                    {
                        obj.NormSide = "Right";
                    }
                }
                else if (obj.Category == "Expenses")
                {
                    obj.Statement = "Income Statement";
                    obj.NormSide = "Left";
                    if (obj.Contra)
                    {
                        obj.NormSide = "Right";
                    }

                }
                else if (obj.Category == "Liability")
                {
                    obj.Statement = "Balance Sheet";
                    obj.NormSide = "Right";
                    if (obj.Contra)
                    {
                        obj.NormSide = "Left";
                    }

                }
                else if (obj.Category == "Equity")
                {
                    obj.Statement = "Balance Sheet";
                    obj.NormSide = "Right";
                    if (obj.Contra)
                    {
                        obj.NormSide = "Left";
                    }
                }
                else if (obj.Category == "Revenue")
                {
                    obj.Statement = "Income Statement";
                    obj.NormSide = "Right";
                    if (obj.Contra)
                    {
                        obj.NormSide = "Left";
                    }
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
                return RedirectToAction("Accounts", "Money");
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
            return RedirectToAction("Accounts", "Money");
        }

        [HttpGet]

        public IActionResult AccountLedger(int? id)
        {
            var sortList = _db.Journal_Accounts.ToList();
            var jList = _db.Journalizes.ToList();

            foreach (var s in sortList)
            {
                foreach (var j in jList)
                {
                    if (s.JournalId == j.JournalId && j.isApproved == true)
                    {
                        s.IsApproved = true;
                        s.Description = j.Description;
                        s.Type = j.Type;
                    }
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var accountmatch = _db.Account.FirstOrDefault(u => u.AccountNumber == id);
            List<Journal_Accounts> ja = new List<Journal_Accounts>();
            foreach (var item in sortList)
            {
                if (item.IsApproved)
                {
                    ja.Add(item);
                }
            }

            AccountLedger account_ledger = new AccountLedger
            {
                account = accountmatch,
                journal_accounts = ja
            };
            return View(account_ledger);

        }
        [HttpPost]
        public IActionResult AccountLedger(DateTime dateSearch1,
           DateTime dateSearch2, double LedgerID)
        {
            List<Journal_Accounts> approved_results = new List<Journal_Accounts>();
            var sortList = _db.Journal_Accounts.ToList();
            var jList = _db.Journalizes.ToList();
            AccountLedger account_ledger = new AccountLedger();
            var accounts = _db.Account.ToList();
            int counter = 0;

            //selects the account for the ledger
            foreach (var item in accounts)
            {
                if (item.AccountNumber == LedgerID)
                {
                    account_ledger.account = item;
                    break;
                }
            }
            //determines original length of list before filtering
            foreach (var s in sortList)
            {
                foreach (var j in jList)
                {
                    if (s.JournalId == j.JournalId && j.isApproved == true && s.AccountName1 == account_ledger.account.AccountName)
                    {
                        counter++;
                    }
                    if (s.JournalId == j.JournalId && j.isApproved == true && s.AccountName2 == account_ledger.account.AccountName)
                    {
                        counter++;
                    }
                }
            }

            //returns search results
            var searchresult = SearchResult(dateSearch1, dateSearch2);
            foreach (var s in searchresult)
            {
                foreach (var j in jList)
                {
                    if (s.JournalId == j.JournalId && j.isApproved == true && s.AccountName1 == account_ledger.account.AccountName)
                    {
                        s.Description = j.Description;
                        s.Type = j.Type;
                        approved_results.Add(s);
                    }
                    if (s.JournalId == j.JournalId && j.isApproved == true && s.AccountName2 == account_ledger.account.AccountName)
                    {
                        s.Description = j.Description;
                        s.Type = j.Type;
                        approved_results.Add(s);
                    }
                }
            }
            account_ledger.journal_accounts = approved_results;
            ViewBag.Search1 = dateSearch1;
            ViewBag.Search2 = dateSearch2;
            ViewBag.Counter = counter;
            return View(account_ledger);
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



        public IActionResult JournalIndex()
        {
            var sortList = _db.Journal_Accounts.ToList();
            return View(sortList);

        }

        public IEnumerable<AccountDB> SearchResult(DateTime date1, DateTime date2, float balance1, float balance2)
        {
            var list = _db.Account.ToList();

            List<AccountDB> activeList = new List<AccountDB>();
            List<AccountDB> resultList = new List<AccountDB>();

            foreach (var item in list)
            {
                if ((item.ChartOfAccounts) && (item.Active))
                {
                    activeList.Add(item);
                }
            }

            if (date1.ToString() != "1/1/0001 12:00:00 AM")
            {
                if (date2.ToString() == "1/1/0001 12:00:00 AM")
                {
                    date2 = DateTime.Now;
                }
                foreach (var item in activeList)
                {
                    if (date1 <= item.CreatedOn && item.CreatedOn <= date2)
                    {
                        resultList.Add(item);
                    }
                }
            }
            if ((balance1 >= 0) && (balance2 == 0))
            {
                foreach (var item in activeList)
                {
                    if (balance1 <= item.Balance)
                    {
                        if (!resultList.Contains(item))
                        {
                            resultList.Add(item);
                        }

                    }
                }
            }
            if ((balance1 == 0) && (balance2 > 0))
            {
                foreach (var item in activeList)
                {
                    if (item.Balance <= balance2 && item.Balance >= 0)
                    {
                        if (!resultList.Contains(item))
                        {
                            resultList.Add(item);
                        }
                    }
                }
            }
            if ((balance1 > 0) && (balance2 > 0))
            {
                foreach (var item in activeList)
                {
                    if ((balance1 <= item.Balance) && (item.Balance <= balance2))
                    {
                        if (!resultList.Contains(item))
                        {
                            resultList.Add(item);
                        }
                    }
                }
            }
            return resultList;
        }
        public IEnumerable<Journal_Accounts> SearchResult(DateTime date1, DateTime date2)
        {

            List<Journal_Accounts> activeList = new List<Journal_Accounts>();
            List<Journal_Accounts> resultList = new List<Journal_Accounts>();

            var jaList = _db.Journal_Accounts.ToList();
            var jList = _db.Journalizes.ToList();

            foreach (var s in jaList)
            {
                activeList.Add(s);
            }

            if (date1.ToString() != "1/1/0001 12:00:00 AM")
            {
                if (date2.ToString() == "1/1/0001 12:00:00 AM")
                {
                    date2 = DateTime.Now;
                }
                foreach (var item in activeList)
                {
                    if (date1 <= item.CreatedOn && item.CreatedOn <= date2)
                    {
                        resultList.Add(item);
                    }
                }
            }
            return resultList;
        }
    }
}
