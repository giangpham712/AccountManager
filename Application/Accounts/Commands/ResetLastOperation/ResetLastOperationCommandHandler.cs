using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Machines.Commands;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.ResetLastOperation
{
    public class ResetLastOperationCommandHandler : CommandHandlerBase<ResetLastOperationCommand, Unit>
    {
        public ResetLastOperationCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(ResetLastOperationCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            var activeOperation = await Context.Set<Operation>().OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(x => x.MachineId == command.Id && x.Active, cancellationToken);

            if (activeOperation == null)
                throw new CommandException($"MachineDto {command.Id} has no active operation");

            Context.Set<Operation>().Remove(activeOperation);

            machine.Turbo = true;

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.ResetOperation,
                Params = activeOperation.TypeName
            }, cancellationToken);

            return await Task.FromResult(Unit.Value);
        }
    }
}