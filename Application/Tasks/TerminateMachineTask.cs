using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Machines.Commands;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application.Tasks
{
    public class TerminateMachineTask : MachineTaskBase<MachineTaskArgsBase>
    {
        public TerminateMachineTask(MachineTaskArgsBase taskArgs) : base(taskArgs)
        {
        }

        public override TaskType Type => TaskType.TerminateMachine;

        public override async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ICloudStateDbContext>();
            var machine = await context.Set<Machine>()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(m => m.Id == MachineId);

            if (machine == null)
            {
                Status = TaskStatus.Failed;
                return;
            }

            machine.SetOperationModeToNormal();
            machine.Terminate = true;
            machine.Turbo = true;

            await machine.Account.SetLastUserCycle(context);

            await context.SaveChangesAsync();

            var mediator = serviceProvider.GetService<IMediator>();
            await mediator.Publish(new MachineActionTriggeredEvent
            {
                User = User,
                Machine = machine,
                Action = UserOperationTypes.TerminateMachine
            });

            while (Status != TaskStatus.Completed && Status != TaskStatus.Failed)
            {
                var cloudInstance = context.Set<CloudInstance>()
                    .AsNoTracking()
                    .OrderByDescending(x => x.Timestamp)
                    .FirstOrDefault(x => x.MachineId == MachineId);

                if (cloudInstance == null)
                {
                    Status = TaskStatus.Failed;
                    continue;
                }

                if (cloudInstance.Status == "stopped")
                {
                    Status = TaskStatus.Completed;
                    FinishedAt = DateTimeOffset.Now;
                    continue;
                }

                if (StartedAt.AddSeconds(900) < DateTimeOffset.Now)
                {
                    Status = TaskStatus.Failed;
                    continue;
                }

                await Task.Delay(30000);
            }
        }
    }
}