﻿using Microsoft.AspNetCore.Identity;
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

                if (item.AccountName.Contains(account.AccountName))
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
                ModelState.AddModelError("","Liquidity must be a positive number.");
                return View(account);
            }

            if (account.Initial < 0)
            {
                ModelState.AddModelError("","Initial Balance cannot be negative.");
                return View(account);
            }

            account.CreatedOn = DateTime.Now;
            var result = _db.Account.Add(account);
            await _db.SaveChangesAsync();
            return RedirectToAction("ChartofAccounts", "Admin");
            
        }
        [HttpGet]

        public IActionResult ChartsOfAccounts()
        {
            var sortList = _db.Account.ToList();
            return View(sortList);
            
        }

        [HttpPost]

        public IActionResult ChartsOfAccounts(string searchString)
        {
            var sortList = SearchResult(searchString);
            ViewBag.result = searchString;
            return View(sortList);

        }

        public  IEnumerable<AccountDB> SearchResult (string search)
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
        public IActionResult EditAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditAccount(AccountDB obj)
        {
            return RedirectToAction("EditAccount", "Money");
        }

        [HttpGet]

        public IEnumerable<AccountDB> AccountLedger()
        {
            var Ledger = _db.Account.ToList();

            List<AccountDB> activeList = new List<AccountDB>();

            foreach (var item in Ledger)
            {
                if (item.Equals(item.AccountNumber))
                {
                    activeList.Add(item);
                }
            }
                return activeList;
   
        }
    }
}