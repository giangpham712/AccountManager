using System;

namespace AccountManager.Domain.Entities.Account
{
    public class IdleSchedule : IEntity
    {
        public DateTimeOffset StopAt { get; set; }
        public int ResumeAfter { get; set; }
        public long AccountId { get; set; }

        public Account Account { get; set; }
        public long Id { get; set; }
    }
}