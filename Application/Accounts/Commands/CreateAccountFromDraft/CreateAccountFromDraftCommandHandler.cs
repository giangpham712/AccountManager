using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Services;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Commands.CreateAccountFromDraft
{
    public class CreateAccountFromDraftCommandHandler : CommandHandlerBase<CreateAccountFromDraftCommand, long>
    {
        private readonly IDraftAccountService _draftAccountService;
        public CreateAccountFromDraftCommandHandler(IMediator mediator, ICloudStateDbContext context, IDraftAccountService draftAccountService) : base(mediator, context)
        {
            _draftAccountService = draftAccountService;
        }

        public override async Task<long> Handle(CreateAccountFromDraftCommand command, CancellationToken cancellationToken)
        {
            var draftAccount = _draftAccountService.GetDraftAccount(command.DraftAccountId);
            if (draftAccount == null)
            {
                throw new CommandException();
            }

            var componentConfigs = await Context.Set<ComponentConfig>().ToListAsync(cancellationToken);
            var componentConfigMapByKey = componentConfigs.ToDictionary(x => $"{x.RootKey}.{x.SubKey}", x => x);

            var index = 0;
            foreach (var machine in draftAccount.Machines)
            {
                var machineComponentConfig = command.MachineComponentConfigs[index++];
                var draftMachineComponentConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(machine.Config.ComponentConfigJson);

                foreach (var entry in machineComponentConfig)
                {
                    var componentConfig = componentConfigMapByKey[entry.Key];
                    if (componentConfig.Protected)
                    {
                        continue;
                    }

                    if (!draftMachineComponentConfig.ContainsKey(entry.Key) || 
                        draftMachineComponentConfig[entry.Key] != entry.Value)
                    {
                        draftMachineComponentConfig[entry.Key] = entry.Value;
                    }
                }

                machine.Config.ComponentConfigJson = JsonConvert.SerializeObject(draftMachineComponentConfig);
            }

            Context.Set<Account>().Add(draftAccount);

            await Context.SaveChangesAsync(cancellationToken);

            _draftAccountService.DeleteDraftAccount(command.DraftAccountId);

            if (draftAccount.ClassId != 200)
                await Mediator.Publish(new AccountCreatedEvent
                {
                    User = command.User,
                    Account = draftAccount,
                    Command = command
                }, cancellationToken);

            return draftAccount.Id;
        }
    }
}
