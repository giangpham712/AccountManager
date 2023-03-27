using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Exceptions;
using AccountManager.Common;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.BatchUpdateAccounts
{
    public class BatchUpdateAccountsCommandHandler : CommandHandlerBase<BatchUpdateAccountsCommand, Unit>
    {
        public BatchUpdateAccountsCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(BatchUpdateAccountsCommand command, CancellationToken cancellationToken)
        {
            var accounts = new List<Account>();
            foreach (var accountId in command.AccountIds)
            {
                var account = await Context.Set<Account>()
                    .FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);

                if (account == null)
                    throw new CommandException();

                foreach (var patchable in BatchUpdateAccountsCommand.Patchables)
                {
                    if (!(patchable.GetValue(command) is Patch patch) || !patch.Patchable)
                        continue;

                    var targetProperty = typeof(Account).GetProperty(patchable.Name);
                    if (targetProperty == null)
                        continue;

                    targetProperty.SetValue(account, patch.Value);
                }

                accounts.Add(account);
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            // Add audit log
            await Mediator.Publish(new UserActionNotification
            {
                User = command.User,
                Accounts = accounts.ToArray(),
                Action = "BatchUpdateAccounts",
                Data = new
                {
                    Params = command,
                    Changes = changes
                }
            }, cancellationToken);

            return Unit.Value;
        }
    }
}