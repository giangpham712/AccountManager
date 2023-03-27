using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Common;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.BatchUpdateMachines
{
    public class BatchUpdateMachinesCommandHandler : CommandHandlerBase<BatchUpdateMachinesCommand, Unit>
    {
        public BatchUpdateMachinesCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(BatchUpdateMachinesCommand command, CancellationToken cancellationToken)
        {
            var machines = new List<Machine>();
            foreach (var machineId in command.MachineIds)
            {
                var machine = await Context.Set<Machine>()
                    .FirstOrDefaultAsync(x => x.Id == machineId, cancellationToken);

                if (machine == null) continue;

                foreach (var patchable in BatchUpdateMachinesCommand.Patchables)
                {
                    if (!(patchable.GetValue(command) is Patch patch) || !patch.Patchable)
                        continue;

                    var targetProperty = typeof(Machine).GetProperty(patchable.Name);
                    if (targetProperty == null)
                        continue;

                    targetProperty.SetValue(machine, patch.Value);
                }

                machine.Turbo = true;
                machine.SetOperationModeToNormal();

                machines.Add(machine);
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            // 
            // Add audit log
            await Mediator.Publish(new UserActionNotification
            {
                User = command.User,
                Machines = machines.ToArray(),
                Action = "BatchUpdateMachines",
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