using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swift_Tomes_Accounting.Data;
using Swift_Tomes_Accounting.DbInitialize;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.DbInitialize
{
    public class DbInitialize : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitialize(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public void initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }
            if (_db.Roles.Any(x => x.Name == "Admin")) return;

            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Manager")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Accountant")).GetAwaiter().GetResult();

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "miller4277@gmail.com",
                    CustomUsername = "Admin1",
                    FirstName = "Default",
                    LastName = "Admin",
                    Email = "miller4277@gmail.com",
                    isApproved = true,
                    Role = "Admin",
                    PasswordDate = DateTime.Now
                };
                _userManager.CreateAsync(user, "Admin123!").GetAwaiter().GetResult();
                ApplicationUser User = _db.Users.FirstOrDefault(x => x.Email == "miller4277@gmail.com");
                _userManager.AddToRoleAsync(User, "Admin").GetAwaiter().GetResult();
            }
        }
    }
    }
