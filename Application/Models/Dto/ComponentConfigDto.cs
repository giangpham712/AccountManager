using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Models.Dto
{
    public class ComponentConfigDto
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public string RootKey { get; set; }

        public string SubKey { get; set; }

        public object SaasDefaultValue { get; set; }

        public object OnPremDefaultValue { get; set; }

        public bool SiteSpecific { get; set; }

        public string DataType { get; set; }

        public bool Protected { get; set; }
    }
}
