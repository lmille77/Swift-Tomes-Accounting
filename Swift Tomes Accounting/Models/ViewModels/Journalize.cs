using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class Journalize
    {
        [Key]
        public int JournalId { get; set; }

        public bool isApproved { get; set; }

        [NotMapped]
        public double AccountNumber { get; set; }

        //just used to display in the view, not stored in database
        //[NotMapped]
        //public string AccountName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> AccountList { get; set; }

        public virtual List<Journal_Accounts> Journal_Accounts { get; set; } = new List<Journal_Accounts>();

        public string docUrl { get; set; }

        
        [NotMapped]
        public IFormFile Document { get; set; }


        public string Reason { get; set; }


        public bool IsRejected { get; set; }

        public string Description { get; set; }
       
        public string Type { get; set; }
    }
}
