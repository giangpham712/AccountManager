using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Commands.QueueOperations
{
    public class QueueOperationsCommandHandler : CommandHandlerBase<QueueOperationsCommand, Unit>
    {
        public QueueOperationsCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(QueueOperationsCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            var runningOperation = await Context.Set<Operation>().FirstOrDefaultAsync(
                x => x.MachineId == machine.Id && (x.Active || x.Status.ToLower() == "running"), cancellationToken);
            if (runningOperation != null)
                throw new CommandException(
                    "Unable to queue operation. An operation is already running for this machine.");

            var operationTypeName = command.Operations.FirstOrDefault();
            var operationType = await Context.Set<OperationType>()
                .FirstOrDefaultAsync(x => x.Name == operationTypeName, cancellationToken);

            if (operationType == null || !(operationType.CanBeManual.HasValue && operationType.CanBeManual.Value))
                throw new CommandException();

            var forcedOperation = new Operation
            {
                Type = operationType,
                Active = true,
                Timestamp = DateTimeOffset.Now,
                Status = "FORCED",
                MachineId = command.Id,
                TypeName = operationTypeName
            };

            machine.Turbo = true;
            machine.SetOperationModeToNormal();

            await machine.Account.SetLastUserCycle(Context);

            Context.Set<Operation>().Add(forcedOperation);
            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.QueueOperation,
                Params = forcedOperation.TypeName
            }, cancellationToken);

            return Unit.Value;
        }
    }
}