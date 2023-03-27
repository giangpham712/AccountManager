namespace AccountManager.Application.Tasks
{
    public class RestoreMachineBackupTaskArgs : MachineTaskArgsBase
    {
        public string SiteMasterBackupFile { get; set; }
        public string LauncherBackupFile { get; set; }
    }
}