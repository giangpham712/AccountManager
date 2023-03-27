using System;

namespace AccountManager.Application.Models.Dto
{
    public class StateDto
    {
        public long Id { get; set; }

        public string MachineName { get; set; }
        public string MachineClassName { get; set; }

        public string Launcher { get; set; }
        public CommitDto LauncherCommit { get; set; }

        public string Reporting { get; set; }
        public CommitDto ReportingCommit { get; set; }

        public string PdfExport { get; set; }
        public CommitDto PdfExportCommit { get; set; }

        public string SiteMaster { get; set; }
        public CommitDto SiteMasterCommit { get; set; }

        public string Client { get; set; }
        public CommitDto ClientCommit { get; set; }

        public string RelExport { get; set; }
        public CommitDto RelExportCommit { get; set; }

        public string SqlExport { get; set; }
        public CommitDto SqlExportCommit { get; set; }

        public string Populate { get; set; }
        public CommitDto PopulateCommit { get; set; }

        public string Linkware { get; set; }
        public CommitDto LinkwareCommit { get; set; }

        public string Smchk { get; set; }
        public CommitDto SmchkCommit { get; set; }

        public string Discovery { get; set; }
        public CommitDto DiscoveryCommit { get; set; }

        public string Deployer { get; set; }
        public CommitDto DeployerCommit { get; set; }

        public string FiberSenSys { get; set; }
        public CommitDto FiberSenSysCommit { get; set; }

        public string FiberMountain { get; set; }
        public CommitDto FiberMountainCommit { get; set; }

        public string ServiceNow { get; set; }
        public CommitDto ServiceNowCommit { get; set; }

        public string CommScope { get; set; }
        public CommitDto CommScopeCommit { get; set; }

        public long? LibraryFileId { get; set; }
        public FileDto LibraryFile { get; set; }
        public long? AccountLibraryFileId { get; set; }
        public FileDto AccountLibraryFile { get; set; }
        public long[] LibraryFileIds { get; set; }
        public FileDto[] LibraryFiles { get; set; }

        public bool Desired { get; set; }
        public bool Locked { get; set; }
        public bool SslEnabled { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public string SiteMasterBackup { get; set; }
        public string LauncherBackup { get; set; }
        public bool MonitoringEnabled { get; set; }
    }
}