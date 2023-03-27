namespace AccountManager.Domain
{
    public class EntityChangeLog
    {
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public long EntityId { get; set; }
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}