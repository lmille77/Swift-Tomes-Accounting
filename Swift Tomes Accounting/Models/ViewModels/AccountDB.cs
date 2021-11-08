using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class AccountDB
    {

        [Key]
        [Display(Name = "Account Number")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public double AccountNumber { get; set; }
       
        [Required]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public string Description { get; set; }
       
        public string NormSide { get; set; }
        [Required]
        public string Category { get; set; }
        public string SubCategory { get; set; }
        [DataType(DataType.Currency)]
        public double Initial { get; set; }
        [DataType(DataType.Currency)]
        public double Debit { get; set; }
        [DataType(DataType.Currency)]
        public double Credit { get; set; }
        [DataType(DataType.Currency)]
        public double Balance { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
        [Required]
        public int Order { get; set; }
        public string Statement { get; set; }
        public string Comments { get; set; }
        public bool Active { get; set; }
        public bool Contra { get; set; }
        public bool ChartOfAccounts { get; set; }

        //[ForeignKey("AccountNumber")]
        //public ICollection<Journalize> Journalize { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> Accounts { get; set; }


        [NotMapped]
        public double TotalRev { get; set; }

        [NotMapped]
        public double TotalEx { get; set; }
    }
}
