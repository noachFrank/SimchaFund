using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.data
{
    public class SimchaForDisplay
    {
        public int Id { get; set;
        }
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int Donors { get; set; }

        public int Donations { get; set; }
    }
}
