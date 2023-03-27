namespace AccountManager.Application.Machines.Commands.ForceBackupForMachine
{
    public class ForceBackupForMachineCommand : CommandBase
    {
        public long Id { get; set; }
        public int TimeToBackup { get; set; }
    }
}