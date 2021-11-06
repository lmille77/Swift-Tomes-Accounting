using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Swift_Tomes_Accounting.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDB>().HasKey(a => new { a.AccountNumber });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }  
        public DbSet<AccountDB> Account { get; set; }
        public DbSet<EventUser> EventUser { get; set; }
        public DbSet<EventAccount> EventAccount { get; set; }
        public DbSet<Journalize> Journalizes { get; set; }
        public DbSet<Journal_Accounts> Journal_Accounts { get; set; }
        public DbSet<ErrorTable> ErrorTable { get; set; }  

    }
   
}
