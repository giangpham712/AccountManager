using System;
using System.Threading.Tasks;

namespace AccountManager.Application.Tasks
{
    public class UpdateMachineSoftwaresTask : MachineTaskBase<MachineTaskArgsBase>
    {
        public UpdateMachineSoftwaresTask(MachineTaskArgsBase taskArgs) : base(taskArgs)
        {
        }

        public override TaskType Type => TaskType.UpdateMachineSoftwares;

        public override async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
        }
    }
}