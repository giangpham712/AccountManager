using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application.Tasks
{
    public class ChangeMachineInstanceTypeTask : MachineTaskBase<ChangeMachineInstanceTypeTaskArgs>
    {
        public ChangeMachineInstanceTypeTask(ChangeMachineInstanceTypeTaskArgs taskArgs) :
            base(taskArgs)
        {
        }

        public override TaskType Type => TaskType.ChangeMachineInstanceType;

        public override async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ICloudStateDbContext>();
            var machine = await context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == MachineId);
            if (machine == null)
            {
                Status = TaskStatus.Failed;
                return;
            }

            machine.CloudInstanceTypeId = TaskArgs.InstanceTypeId;

            await context.SaveChangesAsync();

            while (Status != TaskStatus.Completed && Status != TaskStatus.Failed)
            {
                var operations = context.Set<Operation>().AsNoTracking()
                    .Where(o => o.Timestamp > StartedAt)
                    .OrderByDescending(o => o.Timestamp);


                await Task.Delay(60000);
            }
        }
    }

    public class ChangeMachineInstanceTypeTaskArgs : MachineTaskArgsBase
    {
        public long InstanceTypeId { get; set; }
    }
}