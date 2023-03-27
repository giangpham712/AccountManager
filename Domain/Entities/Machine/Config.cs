using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Domain.Entities.Machine
{
    public class Config : IEntity
    {
        public long MachineId { get; set; }

        public long Id
        {
            get => MachineId;
            set => MachineId = value;
        }

        public string ComponentConfigJson { get; set; }
        public string DeployerConfigJson { get; set; }

        public Machine Machine { get; set; }
    }
}
