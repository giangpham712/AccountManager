namespace AccountManager.Domain.Entities.Account
{
    public class Customer : IEntity
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}