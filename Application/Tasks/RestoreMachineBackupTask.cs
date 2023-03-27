using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Machines.Commands;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application.Tasks
{
    public class RestoreMachineBackupTask : MachineTaskBase<RestoreMachineBackupTaskArgs>
    {
        public RestoreMachineBackupTask(RestoreMachineBackupTaskArgs taskArgs) :
            base(taskArgs)
        {
        }

        public override TaskType Type => TaskType.RestoreMachineBackup;

        public override async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ICloudStateDbContext>();
            var machine = await context.Set<Machine>()
                .Include(x => x.Account)
                .Include(x => x.CloudInstances)
                .FirstOrDefaultAsync(x => x.Id == MachineId);

            if (machine == null) throw new Exception($"Machine with ID {MachineId} doesn't exist");

            var desiredState =
                await context.Set<State>().FirstOrDefaultAsync(x => x.MachineId == MachineId && x.Desired);

            if (desiredState == null)
                throw new Exception("Machine has no desired state");

            BackupProfile launcherBackupProfile = null, siteMasterBackupProfile = null;
            var cloudInstance = machine.CloudInstances.FirstOrDefault();
            if (cloudInstance != null)
            {
                var backupProfiles = await context.Set<BackupProfile>()
                    .Where(x => x.CloudInstanceId == cloudInstance.Id)
                    .ToListAsync();

                launcherBackupProfile = backupProfiles.FirstOrDefault(x => x.AppName == "launcher");
                siteMasterBackupProfile = backupProfiles.FirstOrDefault(x => x.AppName == "sitemaster");
            }

            if (machine.IsLauncher && !TaskArgs.LauncherBackupFile.IsNullOrWhiteSpace())
            {
                desiredState.LauncherBackup = TaskArgs.LauncherBackupFile;
                if (launcherBackupProfile != null && launcherBackupProfile.LastBackup == TaskArgs.LauncherBackupFile)
                {
                    launcherBackupProfile.LastBackup = null;
                }
            }

            if (machine.IsSiteMaster && !TaskArgs.SiteMasterBackupFile.IsNullOrWhiteSpace())
            {
                desiredState.SiteMasterBackup = TaskArgs.SiteMasterBackupFile;
                if (siteMasterBackupProfile != null && siteMasterBackupProfile.LastBackup == TaskArgs.SiteMasterBackupFile)
                {
                    siteMasterBackupProfile.LastBackup = null;
                }
            }

            machine.SetOperationModeToNormal();
            machine.Turbo = true;

            await machine.Account.SetLastUserCycle(context);

            await context.SaveChangesAsync();

            var mediator = serviceProvider.GetService<IMediator>();
            await mediator.Publish(new MachineActionTriggeredEvent
            {
                User = User,
                Machine = machine,
                Action = UserOperationTypes.Restore
            });

            var checkOperations = new List<string>();

            if (machine.IsLauncher) checkOperations.Add(OperationTypes.RestoreLauncher);

            if (machine.IsSiteMaster) checkOperations.Add(OperationTypes.RestoreSiteMaster);

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