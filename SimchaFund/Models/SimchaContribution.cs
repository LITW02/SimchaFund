using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class SimchaContribution
    {
        public bool Included { get; set; }
        public Contributor Contributor { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
    }
}