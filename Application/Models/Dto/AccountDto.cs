using System.Collections.Generic;

namespace AccountManager.Application.Models.Dto
{
    public class AccountDto
    {
        public AccountDto()
        {
            BackupConfig = new BackupConfigDto();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlFriendlyName { get; set; }
        public long? ClassId { get; set; }
        public long? ParentId { get; set; }
        public bool Managed { get; set; }
        public bool AutoTest { get; set; }
        public string AutoTestBranch { get; set; }
        public bool WhiteGlove { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsPublic { get; set; }
        public string Creator { get; set; }
        public string Customer { get; set; }
        public ClassDto Class { get; set; }
        public AccountDto Parent { get; set; }
        public ContactDto Contact { get; set; }
        public BillingDto Billing { get; set; }
        public LicenseConfigDto LicenseConfig { get; set; }
        public MachineConfigDto MachineConfig { get; set; }
        public BackupConfigDto BackupConfig { get; set; }
        public ICollection<IdleScheduleDto> IdleSchedules { get; set; }
        public ICollection<SiteDto> Sites { get; set; }
    }
}