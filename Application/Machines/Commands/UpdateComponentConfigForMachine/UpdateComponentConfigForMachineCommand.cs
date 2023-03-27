using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Machines.Commands.UpdateComponentConfigForMachine
{
    public class UpdateComponentConfigForMachineCommand : CommandBase
    {
        public long Id { get; set; }

        public Dictionary<string, object> ComponentConfig { get; set; }
    }
}
