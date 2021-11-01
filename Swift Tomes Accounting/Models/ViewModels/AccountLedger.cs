using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class AccountLedger
    {
        public AccountDB account;
        public IEnumerable<Journal_Accounts> journal_accounts;
    }
}
