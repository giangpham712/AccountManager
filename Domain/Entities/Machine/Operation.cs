using System;

namespace AccountManager.Domain.Entities.Machine
{
    public class Operation : IEntity
    {
        public long MachineId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int FailCounter { get; set; }
        public string Params { get; set; }
        public int FallbackLevel { get; set; }
        public string TypeName { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? FinishTime { get; set; }
        public int OperationMode { get; set; }
        public string Output { get; set; }
        public string DeployerLog { get; set; }

        public Machine Machine { get; set; }
        public OperationType Type { get; set; }
        public long Id { get; set; }
    }
}