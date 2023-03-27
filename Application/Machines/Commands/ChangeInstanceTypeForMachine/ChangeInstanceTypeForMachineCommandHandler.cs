using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Commands.ChangeInstanceTypeForMachine
{
    public class
        ChangeInstanceTypeForMachineCommandHandler : CommandHandlerBase<ChangeInstanceTypeForMachineCommand, Unit>
    {
        public ChangeInstanceTypeForMachineCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(
            mediator, context)
        {
        }

        public override async Task<Unit> Handle(ChangeInstanceTypeForMachineCommand command,
            CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            machine.CloudInstanceTypeId = command.InstanceTypeId;
            machine.SetOperationModeToNormal();
            machine.Turbo = true;

            await machine.Account.SetLastUserCycle(Context);

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.ChangeInstanceType
            }, cancellationToken);

            return Unit.Value;
        }
    }
}