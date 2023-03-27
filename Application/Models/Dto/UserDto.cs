using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Models.Dto
{
    public class UserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public object ProviderUserKey { get; set; }
        public string UserName { get; set; }
        public string[] Permissions { get; set; }
    }
}
