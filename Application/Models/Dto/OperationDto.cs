using System;

namespace AccountManager.Application.Models.Dto
{
    public class OperationDto
    {
        public long Id { get; set; }
        public long MachineId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int FailCounter { get; set; }
        public string Params { get; set; }
        public int FallbackLevel { get; set; }
        public OperationTypeDto Type { get; set; }
        public string TypeName { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
        public DateTimeOffset? FinishTime { get; set; }

        public TimeSpan RunningTime
        {
            get
            {
                if (FinishTime.HasValue)
                {
                    return (FinishTime.Value - Timestamp);
                }
                else
                {
                    return TimeSpan.FromSeconds(Math.Round((DateTimeOffset.Now - Timestamp).TotalSeconds));
                }
            }
        }
        public int OperationMode { get; set; }

        public string MachineName { get; set; }
        public string MachineClassName { get; set; }

        public long Timeout { get; set; }
        public string OperationTypeName { get; set; }
        public string OperationTypeDescription { get; set; }

        public bool Pending => Params?.ToLower() == "pending";
    }
}