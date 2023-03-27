using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;

namespace AccountManager.Application.Accounts.Commands.UpdateAccount
{
    public class UpdateAccountCommand : CommandBase
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ClassId { get; set; }
        public bool Managed { get; set; }
        public bool AutoTest { get; set; }
        public string AutoTestBranch { get; set; }
        public bool WhiteGlove { get; set; }

        public string Customer { get; set; }

        #region Contact

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone1 { get; set; }
        public string ContactPhone2 { get; set; }

        #endregion

        #region Billing

        public BillingPeriod BillingPeriod { get; set; }
        public double BillingAmount { get; set; }

        #endregion
    }
}