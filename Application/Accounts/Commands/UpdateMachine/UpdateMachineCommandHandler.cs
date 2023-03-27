using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Machines.Commands;
using AccountManager.Common;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateMachine
{
    public class UpdateMachineCommandHandler : CommandHandlerBase<UpdateMachineCommand, Unit>
    {
        public UpdateMachineCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(UpdateMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Machine>()
                .Include(x => x.Account)
                .Include(x => x.CloudInstances)
                .FirstOrDefaultAsync(x => x.Id == command.MachineId, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.MachineId);


            var patches = new List<Patch>();
            foreach (var patchable in UpdateMachineCommand.Patchables)
            {
                if (!(patchable.GetValue(command) is Patch patch) || !patch.Patchable)
                    continue;

                var targetProperty = typeof(Machine).GetProperty(patchable.Name);
                if (targetProperty == null)
                    continue;

                patch.Name = patchable.Name;

                patches.Add(patch);

                if (patchable.Name == "Managed" && machine.Managed == false && (bool?)patch.Value == true)
                {
                    machine.SetOperationModeToNormal();
                }
                else if (patchable.Name == "SampleDataFile")
                {
                    machine.Turbo = true;
                    foreach (var instance in machine.CloudInstances) instance.SiteMasterPopulated = false;
                }

                targetProperty.SetValue(machine, patch.Value);
            }

            var account = machine.Account;
            await account.SetLastUserCycle(Context);

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            await Mediator.Publish(new MachineUpdatedEvent
            {
                User = command.User,
                Machine = machine,
                Updates = patches.ToDictionary(x => x.Name, x => x.Value)
            }, cancellationToken);

            // Add audit log
            await Mediator.Publish(new UserActionNotification
            {
                User = command.User,
                Machines = new[] { machine },
                Action = "UpdateMachine",
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