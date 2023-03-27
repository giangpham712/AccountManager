using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AccountManager.Domain;

namespace AccountManager.Application.Services
{
    public interface IBuildFileService
    {
        Task<IEnumerable<BuildFile>> ListBuildFiles(Software software, BuildFileType? type = null);
        Task<BuildFile> GetBuildFile(string key);
    }

    public class BuildFile
    {
        public BuildFile(string name, string url, long size, DateTime lastModified, BuildFileType type)
        {
            Name = name;
            Url = url;
            Size = size;
            LastModified = lastModified;
            Type = type;

            var parts = Name.Replace(".zip", "").Split(new[] { '-' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 3)
            {
                Date = DateTime.ParseExact(parts[1], "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None);
                Hash = parts[2];
            }
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public BuildFileType Type { get; set; }

        public DateTime Date { get; set; }

        public string Hash { get; }

        public string Release { get; }
    }

    public enum BuildFileType
    {
        Daily,
        Release
    }
}