using System;
using System.Collections.Generic;
using System.Linq;
using AccountManager.Common.Extensions;

namespace AccountManager.Application.Models.Dto
{
    public class MachineDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Params { get; set; }
        public string LauncherUrl { get; set; }
        public string SiteName { get; set; }
        public string MailTo { get; set; }
        public string SiteMasterUrl { get; set; }
        public bool Stop { get; set; }
        public bool Terminate { get; set; }
        public bool NeedsAdmin { get; set; }
        public int FailCounter { get; set; }
        public bool? Dummy { get; set; }
        public string RdpUsers { get; set; }
        public long CloudInstanceTypeId { get; set; }
        public CloudInstanceTypeDto CloudInstanceType { get; set; }
        public bool IsLauncher { get; set; }
        public bool IsSiteMaster { get; set; }
        public bool? Managed { get; set; }
        public bool? ManualMaintenance { get; set; }
        public bool? CreationMailSent { get; set; }
        public string Region { get; set; }
        public string VmImageName { get; set; }
        public bool? Idle { get; set; }
        public bool? Turbo { get; set; }

        public long? AccountId { get; set; }
        public long? ClassId { get; set; }
        public bool? SampleData { get; set; }

        public bool RunSmokeTest { get; set; }
        public string SampleDataFile { get; set; }

        public DateTimeOffset? NextStartTime { get; set; }
        public DateTimeOffset? NextStopTime { get; set; }
        public DateTimeOffset? NextBackupTime { get; set; }
        public DateTimeOffset? AutoStopTime { get; set; }

        public int OperationMode { get; set; }
        public bool OverseeTermination { get; set; }

        public string[] CloudBackupsLauncher { get; set; }
        public string[] CloudBackupsSiteMaster { get; set; }

        public AccountDto Account { get; set; }
        public ClassDto Class { get; set; }
        public ICollection<StateDto> States { get; set; }
        public ICollection<OperationDto> Operations { get; set; }
        public ICollection<OperationDto> PendingOperations { get; set; }
        public ICollection<CloudInstanceDto> CloudInstances { get; set; }
        public List<AMTaskDto> Tasks { get; set; }

        public string BundleVersion { get; set; }

        public ConfigDto Config { get; set; }

        public string GrafanaUrl
        {
            get
            {
                var cloudInstance = CloudInstances.FirstOrDefault();
                if (cloudInstance == null || cloudInstance.GrafanaUid.IsNullOrWhiteSpace())
                    return null;

                var path = (IsLauncher ? LauncherUrl : SiteMasterUrl).Replace(".", "-");

                return $"http://grafana.planetassociates.net/d/{cloudInstance.GrafanaUid}/{path}";
            }
        }
    }
}