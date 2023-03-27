using System;

namespace AccountManager.Application.Accounts.Commands.UpdateBackupSettings
{
    public class UpdateBackupSettingsCommand : CommandBase
    {
        public long AccountId { get; set; }

        public int DailyDaysToRetain { get; set; }
        public string DailyEolAction { get; set; }

        public int WeeklyBackupDay { get; set; }
        public int WeeklyDaysToRetain { get; set; }
        public string WeeklyEolAction { get; set; }


        public string MonthlyEolAction { get; set; }
        public int MonthlyBackupDay { get; set; }
        public int MonthlyDaysToRetain { get; set; }

        public DateTimeOffset[] Times { get; set; }
    }
}