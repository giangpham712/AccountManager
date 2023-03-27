namespace AccountManager.Application.Models.Dto
{
    public class RedisCloudInstanceTypeDto
    {
        public long Id { get; set; }
        public string CloudCode { get; set; }
        public int MemorySize { get; set; }
        public string Name { get; set; }
        public int? CloudCreditCost { get; set; }
    }
}