namespace SimchaFund.data
{
    public class Contributor
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public int Balance { get; set; }

        public bool AlwaysInclude { get; set; }

        public DateTime Date { get; set; }

        public int DonationAmount { get; set; }

        public bool Donated { get; set; }

    }
}