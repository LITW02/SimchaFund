namespace SimchaFund.Models
{
    public class SimchaContributionPost
    {
        public int ContributorId { get; set; }
        public bool Include { get; set; }
        public decimal Amount { get; set; }
    }
}