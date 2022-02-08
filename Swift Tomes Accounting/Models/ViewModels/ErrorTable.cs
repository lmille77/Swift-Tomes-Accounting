using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class ErrorTable
    {
        [Key]
        public int ID { get; set; }
        public string Message { get; set; }
    }
}
