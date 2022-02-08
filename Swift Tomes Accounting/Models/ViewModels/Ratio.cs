using System.ComponentModel.DataAnnotations;

namespace Swift_Tomes_Accounting.Models.ViewModels
{
    public class Ratio
    {
        [Key]
        public int Id { get; set; }

        public double Current { get; set; }

        public double RoA { get; set; }

        public double RoE { get; set; }

        public double Margin { get; set; }

        public double Turnover { get; set; }

        public double Quick { get; set; }

    }
}
