using System.Collections.Generic;

namespace AccountManager.Application.SoftwareUpdate.ValidateSoftwareUpdate
{
    public class ValidateSoftwareUpdateCommand : CommandBase<ValidateSoftwareUpdateResult>
    {
        public long AccountId { get; set; }

        public string LauncherVersionMode { get; set; }
        public string LauncherVersion { get; set; }

        public string SiteMasterVersionMode { get; set; }
        public string SiteMasterVersion { get; set; }

        public string ClientVersionMode { get; set; }
        public string ClientVersion { get; set; }

        public string ReportingVersionMode { get; set; }
        public string ReportingVersion { get; set; }

        public string PdfExportVersionMode { get; set; }
        public string PdfExportVersion { get; set; }

        public string SqlExportVersionMode { get; set; }
        public string SqlExportVersion { get; set; }

        public string PopulateVersionMode { get; set; }
        public string PopulateVersion { get; set; }

        public string DeployerVersionMode { get; set; }
        public string DeployerVersion { get; set; }

        public string LinkwareVersionMode { get; set; }
        public string LinkwareVersion { get; set; }

        public string SmchkVersionMode { get; set; }
        public string SmchkVersion { get; set; }

        public string DiscoveryVersionMode { get; set; }
        public string DiscoveryVersion { get; set; }

        public string MainLibraryMode { get; set; }
        public long[] MainLibraryFiles { get; set; }
        public string AccountLibraryMode { get; set; }
        public long? AccountLibraryFile { get; set; }
    }

    public class ValidationIssue
    {
        public string Description { get; set; }
    }

    public class ValidateSoftwareUpdateResult
    {
        public IEnumerable<ValidationIssue> Issues { get; set; }
    }
}