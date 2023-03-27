namespace AccountManager.Application.Accounts.Commands.CreateMachine
{
    public class CreateMachineCommand : CommandBase
    {
        public long AccountId { get; set; }
        public long? SiteId { get; set; }
        public bool IsLauncher { get; set; }
    }
}