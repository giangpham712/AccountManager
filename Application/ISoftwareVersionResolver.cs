using AccountManager.Domain;

namespace AccountManager.Application
{
    public interface ISoftwareVersionResolver
    {
        string GetRepoForSoftware(Software software);

        VersionInfo Resolve(Software software, string versionMode, string version, string currentVersion = null,
            string defaultVersion = null);

        long[] GetLibraryFiles(long[] fileIds, string mode, long[] currentFileIds);
    }
}