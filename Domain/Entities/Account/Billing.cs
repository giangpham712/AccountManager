namespace AccountManager.Domain.Entities.Account
{
    public class Billing : IEntity
    {
        public double Amount { get; set; }
        public BillingPeriod Period { get; set; }

        public Account Account { get; set; }

        public long AccountId { get; set; }
        public long Id { get; set; }
    }
}