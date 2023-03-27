using System.Collections.Generic;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts
{
    public class UpdateSoftwareForAccountsCommand : UpdateSoftwareCommandBase
    {
        public List<long> Accounts { get; set; }
    }
}