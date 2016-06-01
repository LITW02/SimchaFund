using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.Models
{
    public class IndexViewModel
    {
        public IEnumerable<SimchaInfo> SimchaInfos { get; set; }
        public string Message { get; set; }
    }
}