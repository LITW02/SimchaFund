using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class ContributionsViewModel
    {
        public IEnumerable<SimchaContribution> Contributions { get; set; }
        public Simcha Simcha { get; set; }
        public string Message { get; set; }
    }
}