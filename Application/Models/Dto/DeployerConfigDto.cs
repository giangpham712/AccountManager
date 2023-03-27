namespace AccountManager.Application.Models.Dto
{
    public class DeployerConfigDto
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string RootKey { get; set; }

        public string SubKey { get; set; }

        public object DefaultValue { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }
}