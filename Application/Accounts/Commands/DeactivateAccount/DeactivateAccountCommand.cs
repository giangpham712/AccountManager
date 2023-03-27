using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.DeactivateAccount
{
    public class DeactivateAccountCommand : CommandBase
    {
        public long Id { get; set; }
    }
}