using System;

namespace AccountManager.Application.Models.Dto
{
    public class IdleScheduleDto
    {
        public long Id { get; set; }
        public DateTimeOffset StopAt { get; set; }
        public int ResumeAfter { get; set; }

        public AccountDto Account { get; set; }
    }
}