using System;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Tasks;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Commands.ForceBackupForMachine
{
    public class ForceBackupForMachineCommandHandler : CommandHandlerBase<ForceBackupForMachineCommand, Unit>
    {
        private readonly ITaskManager _taskManager;

        public ForceBackupForMachineCommandHandler(IMediator mediator, ICloudStateDbContext context,
            ITaskManager taskManager) : base(mediator, context)
        {
            _taskManager = taskManager;
        }

        public async Task<Unit> HandleAlt(ForceBackupForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            machine.Turbo = true;
            machine.NextBackupTime = DateTimeOffset.Now.Add(new TimeSpan(0, command.TimeToBackup, 0));
            machine.SetOperationModeToNormal();

            await machine.Account.SetLastUserCycle(Context);

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.Backup
            }, cancellationToken);

            return Unit.Value;
        }

        public override async Task<Unit> Handle(ForceBackupForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            await _taskManager.QueueTaskAsync(new BackupMachineTask(new MachineTaskArgsBase())
            {
                MachineId = command.Id,
                User = command.User
            });

            return Unit.Value;
        }
    }
}