using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.Application.Services
{
    public interface IBackupFileService
    {
        Task<IEnumerable<BackupFile>> ListBackupFiles(string accountUrlName, string software = null);
    }

    public class BackupFile
    {
        public string SoftwareName { get; set; }
        public string Name { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}