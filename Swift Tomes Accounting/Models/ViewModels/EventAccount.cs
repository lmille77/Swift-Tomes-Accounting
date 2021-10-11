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
        public string BeforeAccountName { get; set; }

        public double BeforeAccountNumber { get; set; }
        public string BeforeDescription { get; set; }

        public char BeforeNormSide { get; set; }

        public string BeforeCategory { get; set; }
        public string BeforeSubCategory { get; set; }
        [DataType(DataType.Currency)]
        public int BeforeInitial { get; set; }
        [DataType(DataType.Currency)]
        public int BeforeDebit { get; set; }
        [DataType(DataType.Currency)]
        public int BeforeCredit { get; set; }
        [DataType(DataType.Currency)]
        public int BeforeBalance { get; set; }

        public int BeforeUserID { get; set; }
        public int BeforeOrder { get; set; }
        public string BeforeStatement { get; set; }
        public string BeforeComments { get; set; }
        public string AfterAccountName { get; set; }

        public int AfterAccountNumber { get; set; }
        public string AfterDescription { get; set; }

        public char AfterNormSide { get; set; }

        public string AfterCategory { get; set; }
        public string AfterSubCategory { get; set; }
        [DataType(DataType.Currency)]
        public int AfterInitial { get; set; }
        [DataType(DataType.Currency)]
        public int AfterDebit { get; set; }
        [DataType(DataType.Currency)]
        public int AfterCredit { get; set; }
        [DataType(DataType.Currency)]
        public int AfterBalance { get; set; }

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
        public ApplicationUser eventPerformedBy { get; set; }

    }
}
