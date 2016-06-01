using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class HistoryViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public decimal Balance { get; set; }
        public Contributor Contributor { get; set; }
    }
}