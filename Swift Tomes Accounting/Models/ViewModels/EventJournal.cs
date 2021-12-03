using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class EventJournal
    {
        [Key]
        public int eventID { get; set; }
        [NotMapped]
        public Journalize journal { get; set; }
        [NotMapped]
        public IEnumerable<Journal_Accounts> journal_accounts { get; set; }

        public int journalId { get; set; }        
        public string reason { get; set; }
        public bool isApproved { get; set; }
        public bool isDenied { get; set; }

        public DateTime eventTime { get; set; }
        public string eventType { get; set; }
        public string eventPerformedBy { get; set; }
    
    }
}
