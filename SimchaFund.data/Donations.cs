using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.data
{
    public class Donations
    {
        public int ContributorId { get; set; }

        public int Amount { get; set; }

        public bool Include { get; set; }
    }
}
