using System;

namespace AccountManager.Application.Models.Dto
{
    public class AMTaskDto
    {
        public Guid Id { get; set; }

        public string Type { get; set; }
        public long AccountId { get; set; }
        public long? MachineId { get; set; }
        public int Timeout { get; set; }
        public string Status { get; set; }
        public string Progress { get; set; }

        public DateTimeOffset QueuedAt { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }

        public string Description { get; set; }
    }
}