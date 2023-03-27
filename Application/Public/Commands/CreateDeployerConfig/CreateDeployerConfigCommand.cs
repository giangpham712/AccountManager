namespace AccountManager.Application.Public.Commands.CreateDeployerConfig
{
    public class CreateDeployerConfigCommand : CommandBase
    {
        public string RootKey { get; set; }

        public string SubKey { get; set; }

        public string DefaultValue { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }
}
