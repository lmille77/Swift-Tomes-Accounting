using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Controllers
{
    public class AccountantController : Controller
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AccountantController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
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
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Accountant"))
            {
                return View();
            }
            else if(_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
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
                    }
                }
            }

            return View(selected_accounts);
        }
        
        [HttpGet]
        public IActionResult ChartOfAccounts()
        {
            var accountlist = _db.Account.ToList();
            List<AccountDB> CoA_list = new List<AccountDB>();
            foreach (var item in accountlist)
            {
                if (item.ChartOfAccounts && item.Active)
                {
                    CoA_list.Add(item);
                }
            }

            return View(CoA_list);
        }

        [HttpPost]
        public IActionResult ChartOfAccounts(DateTime dateSearch1,
              DateTime dateSearch2, float balanceSearch1, float balanceSearch2)
        {
            var sortList = SearchResult(dateSearch1, dateSearch2, balanceSearch1, balanceSearch2);

            return View(sortList);

        }        
        public IActionResult Report()
        {

            return View();

        }
        [HttpGet]
        public IActionResult Journalize()
        {
            var accountlist = _db.Account.ToList();
            List<SelectListItem> journalize_list = new List<SelectListItem>();
            foreach (var item in accountlist)
            {
                if (item.ChartOfAccounts && item.Active)
                {
                    SelectListItem journal_item = new SelectListItem() 
                    { 
                        Value = item.AccountName, 
                        Text = item.AccountName 
                    };
                    journalize_list.Add(journal_item);
                }
            }

            Journalize journalize = new Journalize
            {
                AccountList = journalize_list
            };

            journalize.Journal_Accounts.Add(new Journal_Accounts() { JAId = 1 });
            return View(journalize);
        }

        [HttpPost]
        public IActionResult Journalize(Journalize journal)
        {
            var accountlist = _db.Account.ToList();
            List<SelectListItem> journalize_list = new List<SelectListItem>();
            foreach (var item in accountlist)
            {
                if (item.ChartOfAccounts && item.Active)
                {
                    SelectListItem journal_item = new SelectListItem()
                    {
                        Value = item.AccountName,
                        Text = item.AccountName
                    };
                    journalize_list.Add(journal_item);
                }
            }
            journal.AccountList = journalize_list;


            if (ModelState.IsValid)
            {
                double totalcredit = 0;
                double totaldebit = 0;
                List<string> accountnames = new List<string>();
                bool accountnameError = false;
                journal.CreatedOn = DateTime.Now;
                string uniqueFileName = GetUploadedFileName(journal);
                journal.docUrl = uniqueFileName;

                var errorList = _db.ErrorTable.ToList();

                foreach (var item in journal.Journal_Accounts)
                {
                    accountnames.Add(item.AccountName1);
                    accountnames.Add(item.AccountName2);
                    totalcredit += item.Credit;
                    totaldebit += item.Debit;
                    item.CreatedOn = DateTime.Now;
                }
                foreach (var item in accountnames)
                {
                    int count = accountnames.Count(c => c == item && c != null);
                    if (count > 1)
                    {
                        accountnameError = true;
                        break;
                    }
                }
                for (int i = 0; i < journal.Journal_Accounts.Count(); i++)
                {
                    if (journal.Journal_Accounts[i].Credit <= 0 && journal.Journal_Accounts[i].Debit <= 0)
                    {
                        ModelState.AddModelError("", errorList[6].Message);
                        return View(journal);
                    }
                    if (journal.Journal_Accounts[i].AccountName1 == null && journal.Journal_Accounts[i].AccountName2 == null)
                    {
                        ModelState.AddModelError("", errorList[18].Message);
                        return View(journal);
                    }
                }
                if (totalcredit != totaldebit)
                {
                    ModelState.AddModelError("", errorList[7].Message);
                    return View(journal);
                }
                else if (accountnameError)
                {
                    ModelState.AddModelError("", errorList[8].Message);
                    return View(journal);
                }
                _db.Journalizes.Add(journal);
                _db.SaveChanges();
                TempData[SD.Success] = "Journal entry submitted";
                return RedirectToAction("JournalIndex", "Accountant");
            }


            return View(journal);

        }
        private string GetUploadedFileName(Journalize journalize)
        {
            string uniqueFileName = null;

            if (journalize.Document != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + journalize.Document.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    journalize.Document.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
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
            foreach(var item in sortList)
            {
                if(item.IsApproved)
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
        public IActionResult JournalIndex()
        {
            var sortList = _db.Journal_Accounts.ToList();
            var jList = _db.Journalizes.ToList();

            foreach (var s in sortList)
            {
                foreach (var j in jList)
                {
                    if (s.JournalId == j.JournalId)
                    {
                        s.Description = j.Description;
                        s.Type = j.Type;
                    }
                    if (s.JournalId == j.JournalId && j.isApproved == true)
                    {
                        s.IsApproved = true;
                    }
                    if (s.JournalId == j.JournalId && j.IsRejected == true)
                    {
                        s.Reason = j.Reason;
                    }

                }
            }
            return View(sortList);
        }

        [HttpPost]
        public IActionResult JournalIndex(DateTime dateSearch1,
            DateTime dateSearch2)
        {
            var sortList = SearchResult(dateSearch1, dateSearch2);
            var jList = _db.Journalizes.ToList();
            foreach (var s in sortList)
            {
                foreach (var j in jList)
                {
                    if (s.JournalId == j.JournalId)
                    {
                        s.Description = j.Description;
                        s.Type = j.Type;
                    }
                    if (s.JournalId == j.JournalId && j.isApproved == true)
                    {
                        s.IsApproved = true;
                    }
                    if (s.JournalId == j.JournalId && j.IsRejected == true)
                    {
                        s.Reason = j.Reason;
                    }

                }
            }
            return View(sortList);
        }

        [HttpGet]

        public IActionResult AcctEventLog(int? id)
        {

            var all_acctevents = _db.EventAccount.ToList();
            List<EventAccount> select_account = new List<EventAccount>();
            foreach (var acct in all_acctevents)
            {
                if (acct.AfterAccountNumber == id)
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
                        resultList.Add(item);
                    }
                }
            }
            if ((balance1 == 0) && (balance2 > 0))
            {
                foreach (var item in activeList)
                {
                    if (item.Balance <= balance2 && item.Balance >= 0)
                    {
                        resultList.Add(item);
                    }
                }
            }
            if ((balance1 > 0) && (balance2 > 0))
            {
                foreach (var item in activeList)
                {
                    if ((balance1 <= item.Balance) && (item.Balance <= balance2))
                    {
                        resultList.Add(item);
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
