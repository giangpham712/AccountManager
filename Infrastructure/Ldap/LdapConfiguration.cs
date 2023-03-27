using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Ldap
{
    public class LdapConfiguration
    {
        public string ServerUrl { get; set; }
        public string BaseDn { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Admin { get; set; }
        public string AdminPassword { get; set; }
    }
}
