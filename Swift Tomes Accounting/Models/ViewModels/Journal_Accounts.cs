using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class Journal_Accounts
    {
        [Key]
        public int JAId { get; set; }


        //[ForeignKey("AccountDB")]//very important
        //public double AccountNumber { get; set; }
        /*public virtual AccountDB AccountDB { get; private set; }*/ //very important 


        [ForeignKey("Journalize")]//very important
        public int JournalId { get; set; }
        public virtual Journalize Journalize { get; private set; } //very important 

        [DataType(DataType.Currency)]
        public double Debit { get; set; }
        
        [DataType(DataType.Currency)]
        public double Credit { get; set; }


        public string AccountName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> AccountList { get; set; }
    }

}
