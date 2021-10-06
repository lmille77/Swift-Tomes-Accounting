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
            if (account.Order < 0)
            {
                ViewBag.ErrorMessage = "Liquidity cannot be negative.";
                return View(account);
            }

            if (account.Initial < 0)
            {
                ViewBag.ErrorMessage = "Initial Balance cannot be negative.";
                return View(account);
            }

            if (User.IsInRole("Admin"))
            {
                account.CreatedOn = DateTime.Now;
                var result = _db.Account.Add(account);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }
            
            else
            {
                ViewBag.ErrorMessage = "User not allowed to create Accounts";
                return View(account);
            }
            
        }

    }
}
