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

        [Required]
        public string AccountName { get; set; }
        [Key]
        public int AccountNumber { get; set; }
        public string Description { get; set; }
        [Required]
        public char NormSide { get; set; }
        [Required]
        public string Category { get; set; }
        public string SubCategory { get; set; }
        [DataType(DataType.Currency)]
        public int Initial { get; set; }
        [DataType(DataType.Currency)]
        public int Debit { get; set; }
        [DataType(DataType.Currency)]
        public int Credit { get; set; }
        [DataType(DataType.Currency)]
        public int Balance { get; set; }
        [DataType(DataType.DateTime)]
        public string CreatedOn { get; set; }
        [Required]
        public int UserID { get; set; }
        public int Order { get; set; }
        public string Statement { get; set; }
        public string Comments { get; set; }
        






    }
}
