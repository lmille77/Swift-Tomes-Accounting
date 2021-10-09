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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountNumber { get; set; }
        [Required]
        public string AccountName { get; set; }
        public string Description { get; set; }
        [Required]
        public char NormSide { get; set; }
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
    }
}
