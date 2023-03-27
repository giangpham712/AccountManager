namespace AccountManager.Application.Machines.Commands.PublishAsSampleData
{
    public class PublishAsSampleDataCommand : CommandBase
    {
        public long Id { get; set; }
        public string FileName { get; set; }
    }
}