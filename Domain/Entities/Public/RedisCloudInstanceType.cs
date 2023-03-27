namespace AccountManager.Domain.Entities.Public
{
    public class RedisCloudInstanceType : IEntity
    {
        public string CloudCode { get; set; }
        public int MemorySize { get; set; }
        public string Name { get; set; }
        public int? CloudCreditCost { get; set; }
        public long Id { get; set; }
    }
}