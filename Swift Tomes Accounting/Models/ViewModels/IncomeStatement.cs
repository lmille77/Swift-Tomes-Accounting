using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class IncomeStatement
    {
        [Key]
        public int IncomeId { get; set; }

        [DataType(DataType.Currency)]
        public double TotalRev { get; set; }


        [DataType(DataType.Currency)]
        public double TotalEx { get; set; }

        [DataType(DataType.Currency)]
        public double Total { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        public virtual List<AccountDB> Accounts { get; set; } = new List<AccountDB>();
    }
}
