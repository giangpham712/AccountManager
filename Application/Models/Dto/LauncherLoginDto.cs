using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Models.Dto
{
    public class LauncherLoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public object FirstName { get; set; }
        public object LastName { get; set; }
    }
}
