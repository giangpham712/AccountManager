namespace AccountManager.Domain.Entities.Account
{
    public class Contact : IEntity
    {
        public string Name { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }

        public Account Account { get; set; }
        public long AccountId { get; set; }
        public long Id { get; set; }
    }
}