using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Commands.RestoreBackupForMachine
{
    public class RestoreBackupForMachineCommandHandler : CommandHandlerBase<RestoreBackupForMachineCommand, Unit>
    {
        private readonly ITaskManager _taskManager;

        public RestoreBackupForMachineCommandHandler(IMediator mediator, ICloudStateDbContext context,
            ITaskManager taskManager) : base(mediator, context)
        {
            _taskManager = taskManager;
        }

        public override async Task<Unit> Handle(RestoreBackupForMachineCommand command,
            CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            await _taskManager.QueueTaskAsync(new RestoreMachineBackupTask(new RestoreMachineBackupTaskArgs
            {
                LauncherBackupFile = command.LauncherBackupFile,
                SiteMasterBackupFile = command.SiteMasterBackupFile
            })
            {
                MachineId = command.Id,
                User = command.User
            });

            return Unit.Value;
        }
    }
}