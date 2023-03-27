using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.StopMachine
{
    public class StopMachineCommandHandler : CommandHandlerBase<StopMachineCommand, Unit>
    {
        private readonly ITaskManager _taskManager;

        public StopMachineCommandHandler(IMediator mediator, ICloudStateDbContext context,
            ITaskManager taskManager) : base(mediator, context)
        {
            _taskManager = taskManager;
        }

        public override async Task<Unit> Handle(StopMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = Context.Set<Machine>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null) throw new EntityNotFoundException(nameof(Machine), command.Id);

            await _taskManager.QueueTaskAsync(new StopMachineTask(new MachineTaskArgsBase())
            {
                MachineId = command.Id,
                User = command.User
            });

            return Unit.Value;
        }
    }
}