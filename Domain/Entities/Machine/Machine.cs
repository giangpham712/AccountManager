using System;
using System.Collections.Generic;
using System.Linq;
using AccountManager.Domain.Entities.Public;

namespace AccountManager.Domain.Entities.Machine
{
    public class Machine : INamedEntity
    {
        public string Params { get; set; }
        public string LauncherUrl { get; set; }
        public string SiteName { get; set; }
        public string MailTo { get; set; }
        public string SiteMasterUrl { get; set; }
        public bool Terminate { get; set; }
        public bool Stop { get; set; }
        public bool NeedsAdmin { get; set; }
        public int FailCounter { get; set; }
        public bool? Dummy { get; set; }
        public int[] RdpUsers { get; set; }
        public long CloudInstanceTypeId { get; set; }
        public bool IsLauncher { get; set; }
        public bool IsSiteMaster { get; set; }
        public bool? Managed { get; set; }
        public bool? ManualMaintenance { get; set; }
        public bool? CreationMailSent { get; set; }
        public string Region { get; set; }
        public string VmImageName { get; set; }
        public bool? Idle { get; set; }
        public bool? Turbo { get; set; }
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

        public long? AccountId { get; set; }

        public Account.Account Account { get; set; }
        public Class Class { get; set; }
        public CloudInstanceType CloudInstanceType { get; set; }
        public ICollection<State> States { get; set; }
        public ICollection<CloudInstance> CloudInstances { get; set; }
        public ICollection<Operation> Operations { get; set; }
        public ICollection<Message> Messages { get; set; }

        public State DesiredState
        {
            get { return States?.FirstOrDefault(x => x.Desired); }
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public Config Config { get; set; }

        public void SetOperationModeToNormal()
        {
            OperationMode = 0;
        }
    }
}