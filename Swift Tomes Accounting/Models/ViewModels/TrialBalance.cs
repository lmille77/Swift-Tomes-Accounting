using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class TrialBalance
    {
        [Key]
        public int TrialId { get; set; }

        [DataType(DataType.Currency)]
        public double TotalDebit { get; set; }


        [DataType(DataType.Currency)]
        public double TotalCredit { get; set; }

        public bool CJE { get; set; }

        public virtual List<AccountDB> Accounts { get; set; } = new List<AccountDB>();
    }
}
