namespace AccountManager.Domain.Entities.Public
{
    public class DeployerConfig : IEntity
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string RootKey { get; set; }

        public string SubKey { get; set; }

        public string DefaultValue { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }
}