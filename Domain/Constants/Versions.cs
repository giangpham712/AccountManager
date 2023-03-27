namespace AccountManager.Domain.Constants
{
    public static class Versions
    {
        public const string None = "N/A";
    }

    public static class VersionModes
    {
        public const string None = "None";
        public const string Latest = "Latest";
        public const string LatestBuild = "LatestBuild";
        public const string Skip = "Skip";
    }

    public static class LibraryFiles
    {
        public static long? None = null;
    }

    public static class LibraryFileModes
    {
        public const string None = "";
        public const string Latest = "Latest";
        public const string LatestDev = "Latest Dev";
        public const string LatestProd = "Latest Prod";
        public const string LatestAccount = "Latest Account";
        public const string Skip = "Skip";
        public const string Select = "Select";
    }
}