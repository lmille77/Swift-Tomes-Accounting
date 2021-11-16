using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class RetainedEarnings
    {
        [Key]
        public int EarningsId { get; set; }

        [DataType(DataType.Currency)]
        public double BeginRE { get; set; }


        [DataType(DataType.Currency)]
        public double NetInc { get; set; }

        [DataType(DataType.Currency)]
        public double Div { get; set; }

        [DataType(DataType.Currency)]
        public double EndRE { get; set; }
    }
}
