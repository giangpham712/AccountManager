using System;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate
{
    public class CreateOrUpdateBackupSettingsTemplateCommand : CommandBase<long>
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public bool IsPublic { get; set; }

        public int DailyDaysToRetain { get; set; }
        public string DailyEolAction { get; set; }

        public int WeeklyBackupDay { get; set; }
        public int WeeklyDaysToRetain { get; set; }
        public string WeeklyEolAction { get; set; }

        public int MonthlyBackupDay { get; set; }
        public string MonthlyEolAction { get; set; }
        public int MonthlyDaysToRetain { get; set; }

        public DateTimeOffset[] Times { get; set; }
    }
}