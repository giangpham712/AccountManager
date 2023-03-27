using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.Application.Services
{
    public interface ISampleDataFileService
    {
        Task<IEnumerable<SampleDataFile>> ListFiles();
    }

    public class SampleDataFile
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
    }
}