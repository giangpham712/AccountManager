using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Machines.Commands;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application.Tasks
{
    public class BackupMachineTask : MachineTaskBase<MachineTaskArgsBase>
    {
        public BackupMachineTask(MachineTaskArgsBase taskArgs) : base(taskArgs)
        {
        }

        public override TaskType Type => TaskType.BackupMachine;

        public override async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ICloudStateDbContext>();
            var machine = await context.Set<Machine>()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == MachineId);

            if (machine == null)
            {
                Status = TaskStatus.Failed;
                return;
            }

            machine.SetOperationModeToNormal();
            machine.Turbo = true;
            machine.NextBackupTime = DateTimeOffset.Now.Add(new TimeSpan(0, 0, 0));

            await machine.Account.SetLastUserCycle(context);

            await context.SaveChangesAsync();

            var mediator = serviceProvider.GetService<IMediator>();
            await mediator.Publish(new MachineActionTriggeredEvent
            {
                User = User,
                Machine = machine,
                Action = UserOperationTypes.Backup
            });

            var checkOperations = new List<string>();

            if (machine.IsLauncher)
            {
                checkOperations.Add(OperationTypes.BackupLauncher);
                checkOperations.Add(OperationTypes.UploadLauncherBackup);
            }

            if (machine.IsSiteMaster)
            {
                checkOperations.Add(OperationTypes.BackupSiteMaster);
                checkOperations.Add(OperationTypes.UploadSiteMasterBackup);
                checkOperations.Add(OperationTypes.RunSiteMasterCheck);
            }

            while (Status != TaskStatus.Completed && Status != TaskStatus.Failed)
            {
                var operations = context.Set<Operation>().AsNoTracking()
                    .Where(o => o.Timestamp > StartedAt)
                    .OrderByDescending(o => o.Timestamp);

                var completedOperations = operations.Where(o => o.Status == "SUCCESS").Select(o => o.TypeName);

                if (!checkOperations.Except(completedOperations).Any())
                {
                    Status = TaskStatus.Completed;
                    continue;
                }

                if (StartedAt.AddSeconds(1800) < DateTimeOffset.Now)
                {
                    Status = TaskStatus.Failed;
                    continue;
                }

                await Task.Delay(60000);
            }
        }
    }
}