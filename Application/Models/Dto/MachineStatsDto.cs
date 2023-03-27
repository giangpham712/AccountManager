using System.Collections.Generic;

namespace AccountManager.Application.Models.Dto
{
    public class MachineStatsDto
    {
        public int Total { get; internal set; }
        public int NeedsAdmin { get; internal set; }
        public int Stop { get; internal set; }
        public int Turbo { get; internal set; }
        public int Managed { get; internal set; }
        public int Unmanaged { get; internal set; }

        public IEnumerable<dynamic> ClassStats { get; internal set; }

        public int TotalCost { get; set; }
        public int RunningCost { get; set; }
    }
}