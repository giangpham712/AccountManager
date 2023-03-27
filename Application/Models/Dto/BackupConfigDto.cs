using System;

namespace AccountManager.Application.Models.Dto
{
    public class BackupConfigDto
    {
        public BackupConfigDto()
        {
            Times = new DateTimeOffset[] { };
            WeeklyBackupDay = 0;
            MonthlyBackupDay = 0;
        }

        public long Id { get; set; }

        public int DailyDaysToRetain { get; set; }
        public BackupEolAction DailyEolAction { get; set; }

        public int WeeklyDaysToRetain { get; set; }
        public BackupEolAction WeeklyEolAction { get; set; }
        public int WeeklyBackupDay { get; set; }

        public int MonthlyDaysToRetain { get; set; }
        public BackupEolAction MonthlyEolAction { get; set; }
        public int MonthlyBackupDay { get; set; }

        public bool IsTemplate { get; set; }
        public bool IsPublic { get; set; }
        public string Creator { get; set; }
        public string Name { get; set; }
        public DateTimeOffset[] Times { get; set; }
    }
}