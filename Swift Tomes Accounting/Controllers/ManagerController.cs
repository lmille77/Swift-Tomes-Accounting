using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult ChartOfAccounts(DateTime dateSearch1,
             DateTime dateSearch2, float balanceSearch1, float balanceSearch2)
        {
            var sortList = SearchResult(dateSearch1, dateSearch2, balanceSearch1, balanceSearch2);

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
            if ((balance1 > 0) && (balance2 == 0))
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
                    if (item.Balance <= balance2)
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



        public IActionResult Report()
        {
           
            return View();

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
            journal.AccountList = _db.Account.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = u.AccountName,
                Text = u.AccountName
            });


            if (ModelState.IsValid)
            {
                double totalcredit = 0;
                double totaldebit = 0;
                List<string> accountnames = new List<string>();
                bool accountnameError = false;
                journal.CreatedOn = DateTime.Now;
                string uniqueFileName = GetUploadedFileName(journal);
                journal.docUrl = uniqueFileName;
                
                foreach (var item in journal.Journal_Accounts)
                {
                    accountnames.Add(item.AccountName1);
                    accountnames.Add(item.AccountName2);
                    totalcredit += item.Credit;
                    totaldebit += item.Debit;
                    item.CreatedOn = DateTime.Now;
                }
                foreach(var item in accountnames)
                {
                    int count = accountnames.Count(c => c == item && c != null);
                    if(count > 1)
                    {
                        accountnameError = true;
                        break;
                    }
                }
                for(int i = 0; i < journal.Journal_Accounts.Count(); i++)
                {
                    if(journal.Journal_Accounts[i].Credit <= 0 && journal.Journal_Accounts[i].Debit <= 0)
                    {
                        ModelState.AddModelError("", "Entered value for a debit or credit must be greater than 0.");
                        return View(journal);
                    }
                }
                if (totalcredit != totaldebit)
                {
                    ModelState.AddModelError("", "The debits and credits are not balanced.");                    
                    return View(journal);
                }
                else if(accountnameError)
                {
                    ModelState.AddModelError("", "The same account can only be used once.");                    
                    return View(journal);
                }
                _db.Journalizes.Add(journal);
                _db.SaveChanges();
                TempData[SD.Success] = "Journal entry submitted";
                return RedirectToAction("Index", "Admin");
            }


            return View(journal);

        }

        public IActionResult JournalIndex()
        {
            var sortList = _db.Journal_Accounts.ToList();
            var jList = _db.Journalizes.ToList();

            foreach(var s in sortList)
            {
                foreach (var j in jList)
                {
                    if(s.JournalId == j.JournalId && j.isApproved == true)
                    {
                        s.IsApproved = true;
                    }
                }
            }


            return View(sortList);

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
            return View("AccountLedger", objFromDb);
        }



        public async Task<IActionResult> Pending()
        {
            var all_entries = _db.Journalizes.ToList();
            

            List<Journalize> unapproved_entries = new List<Journalize>();
            foreach (var entry in all_entries)
            {
                if (entry.isApproved == false)
                {
                    unapproved_entries.Add(entry);
                }
            }
            return View(unapproved_entries);
        }


        [HttpPost]
        public IActionResult DeleteEntry(int JournalId)
        {
            var objFromdb = _db.Journalizes.FirstOrDefault(u => u.JournalId == JournalId);
          
            if (objFromdb == null)
            {
                return NotFound();
            }
           
            _db.Journalizes.Remove(objFromdb);
            _db.SaveChanges();
            TempData[SD.Success] = "User rejected succesfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult ApproveEntry(int JournalId)
        {
            var objFromdb = _db.Journalizes.FirstOrDefault(u => u.JournalId == JournalId);
           
           

       
            if (objFromdb == null)
            {
                return NotFound();
            }

            if (objFromdb.isApproved == false)
            {

                objFromdb.isApproved = true;
                TempData[SD.Success] = "Entry approved successfully.";
            }

            _db.SaveChanges();


            var obj = _db.Journal_Accounts.ToList();

            foreach(var r in obj)
            {
              if(r.JournalId == JournalId)
                {
                    r.IsApproved = true;
                }
            }
            _db.SaveChanges();

            return RedirectToAction("JournalIndex", "Manager");
        }


        [HttpGet]
        public IActionResult DenyEntry(int? JournalId)
        {
         
            var objFromdb = _db.Journalizes.FirstOrDefault(u=>u.JournalId==JournalId);
           


            return View(objFromdb);
        }

        [HttpPost]
        public IActionResult DenyEntry(Journalize Journal)
        {

            if (ModelState.IsValid)
            {
              
                
                var objFromDb = _db.Journalizes.FirstOrDefault(u => u.JournalId == Journal.JournalId);
                
                if(objFromDb.IsRejected == false)
                {
                    objFromDb.Reason = Journal.Reason;
                    objFromDb.IsRejected = true;
                    _db.SaveChanges();
                    TempData[SD.Success] = "Entry has been denied.";
  
                }
               

                var JA = _db.Journal_Accounts.ToList();

                foreach (var r in JA)
                {
                    if (r.JournalId == Journal.JournalId)
                    {
                        r.IsRejected = true;
                    }
                }
                _db.SaveChanges();

                return RedirectToAction("JournalIndex", "Manager");




            }

            
            


            return View(Journal);
        }


        //[HttpGet]

        //public IActionResult Approval(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var objFromDb = _db.Journal_Accounts.FirstOrDefault(u => u.JournalId == id);
        //    if (objFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(objFromDb);

        //}



        //[HttpPost]
        //public IActionResult Approval(Journal_Accounts JA)
        //{
        //    var objFromdb = _db.Journalizes.FirstOrDefault(u => u.JournalId == JA.JournalId);


        //    if (objFromdb == null)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {




        //        _db.SaveChanges();



        //        TempData[SD.Success] = "Journal entry submitted";
        //        return RedirectToAction("Index", "Admin");
        //    }



        //    return View(objFromdb);
        //}






    }


}
