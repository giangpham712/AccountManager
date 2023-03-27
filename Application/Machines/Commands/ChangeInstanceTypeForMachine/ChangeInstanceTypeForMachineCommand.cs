namespace AccountManager.Application.Machines.Commands.ChangeInstanceTypeForMachine
{
    public class ChangeInstanceTypeForMachineCommand : CommandBase
    {
        public long Id { get; set; }
        public long InstanceTypeId { get; set; }
    }
}