using System;
using AccountManager.Application.SoftwareUpdate;

namespace AccountManager.Application.Accounts.Commands.UpdateInstanceSettings
{
    public class UpdateInstanceSettingsCommand : UpdateSoftwareCommandBase
    {
        public long AccountId { get; set; }

        public bool ShowInGrafana { get; set; }
        public bool UseSparePool { get; set; }
        public bool IncludeSampleData { get; set; }
        public string SampleDataFile { get; set; }
        public bool IncludeIrmoData { get; set; }
        public KeyType KeyType { get; set; }
        public string MarketingVersion { get; set; }

        public string VmImageName { get; set; }
        public DateTimeOffset? AutoStopTime { get; set; }

        public bool ForceBackup { get; set; }
    }
}