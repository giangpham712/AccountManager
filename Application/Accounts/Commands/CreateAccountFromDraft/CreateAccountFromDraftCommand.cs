using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Accounts.Commands.CreateAccountFromDraft
{
    public class CreateAccountFromDraftCommand : CommandBase<long>
    {
        public Guid DraftAccountId { get; set; }

        public List<Dictionary<string, object>> MachineComponentConfigs { get; set; }
    }
}
