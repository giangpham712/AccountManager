using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Public.Commands.CreateComponentConfig
{
    public class CreateComponentConfigCommand : CommandBase
    {
        public string RootKey { get; set; }

        public string SubKey { get; set; }

        public string SaasDefaultValue { get; set; }

        public string OnPremDefaultValue { get; set; }

        public bool SiteSpecific { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }
}
