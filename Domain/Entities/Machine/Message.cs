using System;

namespace AccountManager.Domain.Entities.Machine
{
    public class Message : IEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public long ExpiresAfter { get; set; }

        public long MachineId { get; set; }
        public Machine Machine { get; set; }
        public long Id { get; set; }
    }
}