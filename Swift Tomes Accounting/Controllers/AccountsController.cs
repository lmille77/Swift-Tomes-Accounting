using Microsoft.AspNetCore.Mvc;
using Swift_Tomes_Accounting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Swift_Tomes_Accounting.Models.ViewModels;

namespace Swift_Tomes_Accounting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {

        
        private readonly ApplicationDbContext _db;
       

        public AccountsController(ApplicationDbContext db)
        {
            _db = db;
   
        }

       
        public IActionResult GetAccounts()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var accountData = (from tempcustomer in _db.Account select tempcustomer);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    accountData = accountData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
               
                if (!string.IsNullOrEmpty(searchValue))
                {
                    accountData = accountData.Where(m => m.AccountName.Contains(searchValue)
                                                || m.Category.Contains(searchValue)
                                                || m.SubCategory.Contains(searchValue)
                                                || m.UserName.Contains(searchValue));
                }
                recordsTotal = accountData.Count();
                var data = accountData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
