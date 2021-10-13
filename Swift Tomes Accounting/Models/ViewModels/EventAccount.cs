using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class EventAccount
    {
        [Key]
        public int eventID { get; set; }
        public string BeforeAccountName { get; set; }

        public double BeforeAccountNumber { get; set; }
        public string BeforeDescription { get; set; }

        public string BeforeNormSide { get; set; }

        public string BeforeCategory { get; set; }
        public string BeforeSubCategory { get; set; }
        [DataType(DataType.Currency)]
        public double BeforeInitial { get; set; }
        [DataType(DataType.Currency)]
        public double BeforeDebit { get; set; }
        [DataType(DataType.Currency)]
        public double BeforeCredit { get; set; }
        [DataType(DataType.Currency)]
        public double BeforeBalance { get; set; }

        public int BeforeUserID { get; set; }
        public int BeforeOrder { get; set; }
        public string BeforeStatement { get; set; }
        public string BeforeComments { get; set; }
        public string AfterAccountName { get; set; }

        public double AfterAccountNumber { get; set; }
        public string AfterDescription { get; set; }

        public string AfterNormSide { get; set; }

        public string AfterCategory { get; set; }
        public string AfterSubCategory { get; set; }
        [DataType(DataType.Currency)]
        public double AfterInitial { get; set; }
        [DataType(DataType.Currency)]
        public double AfterDebit { get; set; }
        [DataType(DataType.Currency)]
        public double AfterCredit { get; set; }
        [DataType(DataType.Currency)]
        public double AfterBalance { get; set; }

        public int AfterUserID { get; set; }
        public int AfterOrder { get; set; }
        public string AfterStatement { get; set; }
        public string AfterComments { get; set; }
        public bool BeforeisActive { get; set; }
        public bool BeforeisContra { get; set; }
        public bool AfterisActive { get; set; }
        public bool AfterisContra { get; set; }

        public DateTime eventTime { get; set; }
        public string eventType { get; set; }
        public string eventPerformedBy { get; set; }

    }
}
