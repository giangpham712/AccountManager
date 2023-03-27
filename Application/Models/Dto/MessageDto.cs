using System;

namespace AccountManager.Application.Models.Dto
{
    public class MessageDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public long ExpiresAfter { get; set; }

        public long MachineId { get; set; }
        public MachineDto Machine { get; set; }
    }
}