using System;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Constants;

namespace AccountManager.Domain
{
    public class VersionInfo
    {
        public VersionInfo()
        {
        }

        public VersionInfo(string value)
        {
            string version, branchName;
            if (value == Versions.None)
            {
                branchName = null;
                version = null;
            }
            else if (!value.IsNullOrWhiteSpace() && value.Contains("|"))
            {
                var parts = value.Split(new[] { '|' }, StringSplitOptions.None);
                branchName = parts[0].Trim();
                version = parts[1].Trim();
            }
            else
            {
                version = value;
                branchName = null;
            }

            Version = version;
            Branch = branchName;
        }

        public string Version { get; set; }
        public string Branch { get; set; }

        public override string ToString()
        {
            if (!Branch.IsNullOrWhiteSpace()) return $"{Branch}|{Version}";

            return Version;
        }
    }

    public static class VersionInfoExtensions
    {
        public static bool IsNone(this VersionInfo versionInfo)
        {
            if (versionInfo == null)
                return true;

            if (versionInfo.Version.IsNullOrWhiteSpace())
                return true;

            return false;
        }
    }
}