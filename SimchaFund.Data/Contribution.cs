using System;

namespace SimchaFund.Data
{
    public class Contribution
    {
        public DateTime SimchaDate { get; set; }
        public string SimchaName { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
    }
}