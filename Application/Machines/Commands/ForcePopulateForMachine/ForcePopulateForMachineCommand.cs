namespace AccountManager.Application.Machines.Commands.ForcePopulateForMachine
{
    public class ForcePopulateForMachineCommand : CommandBase
    {
        public long Id { get; set; }

        public bool PopulateLauncher { get; set; }
        public bool PopulateSiteMaster { get; set; }
        public string AltString { get; set; }
    }
}