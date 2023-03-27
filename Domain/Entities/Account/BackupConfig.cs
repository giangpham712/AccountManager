using System;

namespace AccountManager.Domain.Entities.Account
{
    public class BackupConfig : ISupportTemplate
    {
        public int DailyDaysToRetain { get; set; }
        public string DailyEolAction { get; set; }

        public int WeeklyBackupDay { get; set; }
        public int WeeklyDaysToRetain { get; set; }
        public string WeeklyEolAction { get; set; }

        public int MonthlyBackupDay { get; set; }
        public int MonthlyDaysToRetain { get; set; }
        public string MonthlyEolAction { get; set; }

        public DateTimeOffset[] Times { get; set; }

        public string Creator { get; set; }

        public Account Account { get; set; }
        public long? AccountId { get; set; }

        public long Id { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsPublic { get; set; }
        public string Name { get; set; }
    }
}