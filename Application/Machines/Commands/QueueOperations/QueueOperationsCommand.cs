namespace AccountManager.Application.Machines.Commands.QueueOperations
{
    public class QueueOperationsCommand : CommandBase
    {
        public long Id { get; set; }
        public string[] Operations { get; set; }
    }
}