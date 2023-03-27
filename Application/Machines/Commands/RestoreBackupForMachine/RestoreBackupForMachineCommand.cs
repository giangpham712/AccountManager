namespace AccountManager.Application.Machines.Commands.RestoreBackupForMachine
{
    public class RestoreBackupForMachineCommand : CommandBase
    {
        public long Id { get; set; }
        public string SiteMasterBackupFile { get; set; }
        public string LauncherBackupFile { get; set; }
    }
}