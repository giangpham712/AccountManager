namespace AccountManager.Application.Public.Commands.UpdateDeployerConfig
{
    public class UpdateDeployerConfigCommand : CommandBase
    {
        public long Id { get; set; }

        public string DefaultValue { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }
}
