using System;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Machines.Commands.ForcePopulateForMachine
{
    public class ForcePopulateForMachineCommandHandler : CommandHandlerBase<ForcePopulateForMachineCommand, Unit>
    {
        public ForcePopulateForMachineCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(
            mediator, context)
        {
        }

        public override async Task<Unit> Handle(ForcePopulateForMachineCommand command,
            CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Account)
                .Include(x => x.CloudInstances)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            if (!command.PopulateSiteMaster && !command.PopulateLauncher) return Unit.Value;

            await Mediator.Publish(new MachineActionTriggeredEvent
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.ForcePopulate,
                Params = JsonConvert.SerializeObject(new
                {
                    command.PopulateLauncher,
                    command.PopulateSiteMaster,
                    command.AltString
                })
            }, cancellationToken);

            if (command.PopulateLauncher)
            {
                var operationType = await Context.Set<OperationType>()
                    .FirstAsync(x => x.Name == OperationTypes.PopulateLauncher, cancellationToken);

                var operation = new Operation
                {
                    Type = operationType,
                    Active = true,
                    Timestamp = DateTimeOffset.Now,
                    Status = "FORCED",
                    MachineId = command.Id,
                    TypeName = operationType.Name
                };

                Context.Set<Operation>().Add(operation);
            }

            if (command.PopulateSiteMaster)
            {
                var operationType = await Context.Set<OperationType>()
                    .FirstAsync(x => x.Name == OperationTypes.PopulateSitemaster, cancellationToken);

                var operation = new Operation
                {
                    Type = operationType,
                    Active = true,
                    Timestamp = DateTimeOffset.Now,
                    Status = "FORCED",
                    MachineId = command.Id,
                    TypeName = operationType.Name
                };

                Context.Set<Operation>().Add(operation);
            }

            machine.SetOperationModeToNormal();
            machine.Turbo = true;

            await machine.Account.SetLastUserCycle(Context);

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}