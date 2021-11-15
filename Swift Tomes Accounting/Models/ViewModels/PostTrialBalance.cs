using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class PostTrialBalance
    {
        [Key]
        public int PostTrialId { get; set; }

        [DataType(DataType.Currency)]
        public double TotalDebit { get; set; }


        [DataType(DataType.Currency)]
        public double TotalCredit { get; set; }

        public bool CJE { get; set; }

        public virtual List<AccountDB> Accounts { get; set; } = new List<AccountDB>();
    }
}
