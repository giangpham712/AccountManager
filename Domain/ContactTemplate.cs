using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;

namespace AccountManager.Domain
{
    public class ContactTemplate
    {
        public string Name { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
    }

    public class BillingTemplate
    {
        public double Amount { get; set; }
        public BillingPeriod Period { get; set; }
    }
}