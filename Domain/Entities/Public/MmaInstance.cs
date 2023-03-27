using System;
using AccountManager.Domain.Entities.Machine;

namespace AccountManager.Domain.Entities.Public
{
    public class MmaInstance : IEntity
    {
        public bool? MmaRunning { get; set; }
        public DateTimeOffset? MmaStartedAt { get; set; }
        public long? MmaCycle { get; set; }
        public long? MmaTimeout { get; set; }
        public long? MmaCycleDividerRegular { get; set; }
        public long? MmaCycleDividerTurbo { get; set; }

        public bool? AmaRunning { get; set; }
        public DateTimeOffset? AmaStartedAt { get; set; }
        public long? AmaTimeout { get; set; }

        public string CommitHash { get; set; }
        public string CommitTimeAgo { get; set; }
        public DateTimeOffset? CommitTimestamp { get; set; }

        public string DeployerBranch { get; set; }
        public string DeployerVer { get; set; }
        public DateTimeOffset? DeployerVerUpdatedAt { get; set; }

        public bool Standby { get; set; }
        public int PriorityGroup { get; set; }
        public string CaaHost { get; set; }

        public Class MachineClass { get; set; }
        public long? MachineClassId { get; set; }
        public long Id { get; set; }
    }
}