using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class Journalize
    {
        [Key]
        public int JournalId { get; set; }

        //public double AccountNumber { get; set; }

        public string Account1 { get; set; }
        public string Account2 { get; set; }

        public int MyProperty { get; set; }

        [DataType(DataType.Currency)]
        public double Debit1 { get; set; }

        [DataType(DataType.Currency)]
        public double Credit1 { get; set; }
        [DataType(DataType.Currency)]
        public double Debit2 { get; set; }

        [DataType(DataType.Currency)]
        public double Credit2 { get; set; }
        [DataType(DataType.Currency)]
        public double Balance { get; set; }

        public string Description { get; set; }

        public bool isApproved { get; set; }


     
    }
}
