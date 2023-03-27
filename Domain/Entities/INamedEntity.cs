namespace AccountManager.Domain.Entities
{
    public interface INamedEntity : IEntity
    {
        string Name { get; set; }
    }
}