using SimchaFund.data;

namespace SimchaFund.web.Models
{
    public class SimchaViewModel
    {
        public string Message { get; set; }

        public List<SimchaForDisplay> Simchas { get; set; }

        public int TotalDonors { get; set; }
    }
}
