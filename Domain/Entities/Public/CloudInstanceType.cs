namespace AccountManager.Domain.Entities.Public
{
    public class CloudInstanceType : IEntity
    {
        public string CloudCode { get; set; }
        public int StorageSize { get; set; }
        public string Name { get; set; }
        public int? CloudCreditCost { get; set; }
        public long Id { get; set; }
    }
}