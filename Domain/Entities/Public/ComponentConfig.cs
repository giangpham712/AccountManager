using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Domain.Entities.Public
{
    public class ComponentConfig : IEntity
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string RootKey { get; set; }

        public string SubKey { get; set; }

        public string SaasDefaultValue { get; set; }

        public string OnPremDefaultValue { get; set; }

        public bool SiteSpecific { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }
}
