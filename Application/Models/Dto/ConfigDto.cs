using System.Collections.Generic;

namespace AccountManager.Application.Models.Dto
{
    public class ConfigDto
    {
        public long MachineId { get; set; }
        public Dictionary<string, object> ComponentConfig { get; set; }
        public Dictionary<string, object> DeployerConfig { get; set; }
    }
}