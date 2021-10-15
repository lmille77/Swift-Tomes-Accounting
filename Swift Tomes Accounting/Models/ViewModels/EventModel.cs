using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class EventModel
    {
        public IEnumerable<EventUser> EventUser { get; set; }
        public IEnumerable<EventAccount> EventAccount { get; set; }

    }
}
