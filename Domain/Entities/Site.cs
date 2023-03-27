namespace AccountManager.Domain.Entities
{
    public class Site
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UrlFriendlyName { get; set; }
        public long CloudInstanceType { get; set; }
        public int Port { get; set; }
        public long AccountId { get; set; }

        public long? MachineId { get; set; }

        public Account.Account Account { get; set; }
        public Machine.Machine Machine { get; set; }
    }
}