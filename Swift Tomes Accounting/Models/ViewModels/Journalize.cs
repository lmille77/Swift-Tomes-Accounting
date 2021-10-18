using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class Journalize
    {
        [Key]
        public int JournalId { get; set; }

        public double AccountNumber { get; set; }
        
        public string Description { get; set; }

        public bool isApproved { get; set; }

        
    }
}
