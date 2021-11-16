using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class BalanceSheet
    {
        [Key]
        public int BalanceId { get; set; }

        [DataType(DataType.Currency)]
        public double TotalAs { get; set; }


        [DataType(DataType.Currency)]
        public double TotalLi { get; set; }

        [DataType(DataType.Currency)]
        public double TotalEQ { get; set; }

        [DataType(DataType.Currency)]
        public double EndRE { get; set; }

        public virtual List<AccountDB> Accounts { get; set; } = new List<AccountDB>();
    }
}

