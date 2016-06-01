using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.Models
{
    public class ContributorIndexViewModel
    {
        public IEnumerable<ContributorInfo> Contributors { get; set; }
        public decimal TotalBalance { get; set; }
        public string Message { get; set; }
    }
}