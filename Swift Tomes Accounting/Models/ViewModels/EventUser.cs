using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class EventUser
    {
        [Key]
        public int eventID { get; set; }
        public string BeforeFname { get; set; }
        public string BeforeLname { get; set; }
        public string BeforeAddress { get; set; }
        public string BeforeuserName { get; set; }
        public bool BeforeisActive { get; set; }
        public string BeforeDOB { get; set; }
        public string BeforeRole { get; set; }


        public string AfterFname { get; set; }
        public string AfterLname { get; set; }
        public string AfterDOB { get; set; }
        public string AfterAddress { get; set; }
        public string AfteruserName { get; set; }
        public bool AfterisActive { get; set; }
        public string AfterRole { get; set; }

        public DateTime eventTime { get; set; }
        public string eventType { get; set; }
        public string eventPerformedBy { get; set; }
        
    }
}
