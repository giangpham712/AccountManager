using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Domain.Entities.Machine
{
    public class BackupProfile : IEntity
    {
        public long Id { get; set; }
        public long CloudInstanceId { get; set; }
        public string[] LocalBackups { get; set; }
        public string LastBackup { get; set; }
        public string LastRestore { get; set; }
        public string AppName { get; set; }
    }
}
