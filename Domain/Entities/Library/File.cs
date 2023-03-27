using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AccountManager.Domain.Entities.Library
{
    public class File
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string AddedBy { get; set; }
        public string Signature { get; set; }
        public double? Size { get; set; }
        public string Manifest { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public ICollection<Package> Packages { get; set; }

        public ReleaseStage ReleaseStage
        {
            get
            {
                var match = Regex.Match(Name, @"@(dev|rel)");
                if (match.Success && match.Value == "@rel")
                    return ReleaseStage.Released;

                return ReleaseStage.Development;
            }
        }

        public int BaseVersion
        {
            get
            {
                int version;
                var basePackage = Packages?.FirstOrDefault(x => x.Name == "base");
                if (basePackage != null) return int.TryParse(basePackage.Version, out version) ? version : 0;
                var match = Regex.Match(Name, @"_base(\d+)");
                return match.Success && int.TryParse(match.Groups[1].Value, out version) ? version : 0;
            }
        }
    }

    public enum ReleaseStage
    {
        Development,
        Released
    }

    public class FileManifest
    {
        public PackageManifest[] Packages { get; set; }

        public static FileManifest FromString(string manifestStr)
        {
            var lines = manifestStr.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return new FileManifest
            {
                Packages = lines.Select(PackageManifest.FromString).ToArray()
            };
        }
    }

    public class PackageManifest
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string InternalVersion { get; set; }
        public DateTimeOffset? LastExportedTime { get; set; }
        public string MarketingVersion { get; set; }
        public SoftwareVersion LCTVersion { get; set; }

        public static PackageManifest FromString(string manifestStr)
        {
            var parts = manifestStr.Replace("\"", "").Split(',');
            return new PackageManifest
            {
                Name = parts[0].Trim(),
                Path = parts[1].Trim(),
                InternalVersion = parts[2].Trim(),
                LastExportedTime = string.IsNullOrWhiteSpace(parts[3].Trim()) ? null : (DateTimeOffset?) DateTimeOffset.Parse(parts[3].Trim()),
                MarketingVersion = parts[4].Trim(),
                LCTVersion = SoftwareVersion.FromString(parts[5].Trim())
            };
        }
    }

    public class SoftwareVersion
    {
        public string Branch { get; set; }
        public string VersionTag { get; set; }
        public string ShortHash { get; set; }
        public DateTime DateTime { get; set; }

        public static SoftwareVersion FromString(string versionStr)
        {
            var parts = versionStr.Split(':');
            return new SoftwareVersion
            {
                Branch = parts[0].Trim(),
                VersionTag = parts[1].Trim(),
                ShortHash = parts[2].Trim(),
                DateTime = DateTime.ParseExact(parts[3].Trim(), "dd-MM-yyyy", null)
            };
        }
    }
}