using System;

namespace AccountManager.Domain.Entities.Public
{
    public class CloudInstance : IEntity
    {
        public string CloudId { get; set; }
        public string HostName { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public DateTimeOffset? TermTimestamp { get; set; }
        public string Status { get; set; }
        public string CloudCode { get; set; }
        public bool Active { get; set; }
        public bool? DnsReady { get; set; }
        public bool? RdpReady { get; set; }
        public int? StorageSize { get; set; }
        public string IpAddress { get; set; }

        public bool LauncherPopulated { get; set; }
        public bool SiteMasterPopulated { get; set; }
        public string AltString { get; set; }

        public string GrafanaUid { get; set; }
        public string VmImageName { get; set; }

        public long MachineId { get; set; }
        public Machine.Machine Machine { get; set; }
        public long Id { get; set; }
    }
}