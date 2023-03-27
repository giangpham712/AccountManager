using System;

namespace AccountManager.Domain.Entities.Machine
{
    public abstract class StateBase : IEntity
    {
        public string Launcher { get; set; }
        public string SiteMaster { get; set; }
        public string Client { get; set; }
        public string PdfExport { get; set; }
        public string RelExport { get; set; }
        public string SqlExport { get; set; }
        public string Reporting { get; set; }
        public string Populate { get; set; }
        public string Linkware { get; set; }
        public string Smchk { get; set; }
        public string Discovery { get; set; }
        public string Deployer { get; set; }
        public string FiberSenSys { get; set; }
        public string FiberMountain { get; set; }
        public string ServiceNow { get; set; }
        public string CommScope { get; set; }
        public long? LibraryFile { get; set; }
        public long? AccountLibraryFile { get; set; }
        public long[] LibraryFiles { get; set; }

        public bool Locked { get; set; }
        public bool SslEnabled { get; set; }
        public bool MonitoringEnabled { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string SiteMasterBackup { get; set; }
        public string LauncherBackup { get; set; }


        public long? MachineId { get; set; }
        public Machine Machine { get; set; }
        public long Id { get; set; }
    }
}