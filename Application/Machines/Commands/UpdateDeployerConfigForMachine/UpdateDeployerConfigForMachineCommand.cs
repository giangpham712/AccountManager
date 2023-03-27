using System.Collections.Generic;

namespace AccountManager.Application.Machines.Commands.UpdateDeployerConfigForMachine
{
    public class UpdateDeployerConfigForMachineCommand : CommandBase
    {
        public long Id { get; set; }

        public Dictionary<string, object> DeployerConfig { get; set; }
    }
}
