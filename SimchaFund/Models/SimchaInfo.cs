using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class SimchaInfo
    {
        public Simcha Simcha { get; set; }
        public int ContributorCount { get; set; }
        public int SimchaContributorCount { get; set; }
        public decimal SimchaContributionAmount { get; set; }
    }
}