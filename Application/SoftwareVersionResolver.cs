using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;

namespace AccountManager.Application
{
    public class SoftwareVersionResolver : ISoftwareVersionResolver
    {
        private static readonly IDictionary<Software, string> RepoBySoftware;
        private readonly ICloudStateDbContext _context;

        static SoftwareVersionResolver()
        {
            var enumType = typeof(Software);
            RepoBySoftware = Enum.GetValues(typeof(Software)).Cast<Software>().ToDictionary(s => s, s =>
            {
                var softwareComponentAttribute = enumType.GetMember(s.ToString()).First()
                    .GetCustomAttribute<SoftwareComponentAttribute>();

                return softwareComponentAttribute.RepoName;
            });
        }

        public SoftwareVersionResolver(ICloudStateDbContext context)
        {
            _context = context;
        }

        public string GetRepoForSoftware(Software software)
        {
            return RepoBySoftware.TryGetValue(software, out var repo) ? repo : null;
        }

        public VersionInfo Resolve(Software software, string versionMode, string version, string currentVersion = null,
            string defaultVersion = null)
        {
            switch (versionMode)
            {
                case null:
                case VersionModes.None:
                    return new VersionInfo(defaultVersion);
                case VersionModes.Latest:
                    var repo = RepoBySoftware[software];

                    string branchName = null;
                    if (!version.IsNullOrWhiteSpace())
                    {
                        var parts = version.Split(new[] { '|' }, StringSplitOptions.None);
                        branchName = parts[0].Trim();
                    }

                    var query = !branchName.IsNullOrWhiteSpace()
                        ? _context.Set<Commit>().Include(x => x.Branch).Where(x => x.Branch.Name == branchName)
                        : _context.Set<Commit>().Include(x => x.Branch).Where(x => x.Branch.Stable);

                    query = query.Where(x => x.Repo == repo);

                    var latestCommit = query.OrderByDescending(x => x.Timestamp).FirstOrDefault();

                    return new VersionInfo { Branch = latestCommit?.Branch.Name, Version = latestCommit?.ShortHash };
                case VersionModes.LatestBuild:
                    return new VersionInfo(defaultVersion);
                case VersionModes.Skip:
                    return new VersionInfo(currentVersion);
                default:
                    return new VersionInfo(version);
            }
        }

        public long[] GetLibraryFiles(long[] fileIds, string mode, long[] currentFileIds)
        {
            switch (mode)
            {
                case LibraryFileModes.Select:
                {
                    var libraryFiles = _context.Set<File>().Where(x => fileIds.Contains(x.Id));
                    return libraryFiles.Select(x => x.Id).ToArray();
                }
                case LibraryFileModes.None:
                    return new long[0];
                case LibraryFileModes.Skip:
                    return currentFileIds;
                case LibraryFileModes.LatestAccount:
                {
                    var libraryFile = _context.Set<File>().OrderByDescending(x => x.Timestamp)
                        .FirstOrDefault(x => x.Type == "Account");
                    return libraryFile == null ? new long[0] : new[] { libraryFile.Id };
                }
                case LibraryFileModes.LatestDev:
                {
                    var libraryFile = _context.Set<File>().OrderByDescending(x => x.Timestamp)
                        .FirstOrDefault(x => x.Type == "Dev");
                    return libraryFile == null ? new long[0] : new[] { libraryFile.Id };
                }
                case LibraryFileModes.LatestProd:
                {
                    var libraryFile = _context.Set<File>().OrderByDescending(x => x.Timestamp)
                        .FirstOrDefault(x => x.Type == "Prod");
                    return libraryFile == null ? new long[0] : new[] { libraryFile.Id };
                }

                default:
                    return new long[0];
            }
        }
    }
}