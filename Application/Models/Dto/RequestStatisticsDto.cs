using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Models.Dto
{
    public class RequestStatisticsDto
    {
        public long Id { get; set; }
        public long MachineId { get; set; }
        public string MachineName { get; set; }
        public string Url { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public float LastLatency { get; set; }
        public float LastSuccess { get; set; }
        public float AvgWeek { get; set; }
        public float AvgDay { get; set; }
        public float AvgHour { get; set; }
        public float SuccWeek { get; set; }
        public float SuccDay { get; set; }
        public float SuccHour { get; set; }
        public long Rcount { get; set; }

        public MachineDto Machine { get; set; }
    }
}
