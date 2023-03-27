namespace AccountManager.Application.Models.Dto
{
    public class AccountStatsDto
    {
        public long Id { get; set; }
        public int NeedsAdmin { get; internal set; }
        public int ManualMaintenance { get; internal set; }
        public int Managed { get; internal set; }
        public int Unmanaged { get; set; }
        public int Turbo { get; internal set; }
        public int MachineCount { get; set; }
        public int Idle { get; set; }
        public int Stopped { get; set; }
    }
}