using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.ActivateAccount
{
    public class ActivateAccountCommand : CommandBase
    {
        public long Id { get; set; }
        public bool StartMachines { get; set; }
    }
}