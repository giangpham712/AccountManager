namespace AccountManager.Domain.Entities
{
    public interface ISupportTemplate : INamedEntity
    {
        bool IsTemplate { get; set; }
        bool IsPublic { get; set; }
    }
}