namespace AccountManager.Domain.Entities.Machine
{
    public static class OperationTypes
    {
        public static string LaunchCloudInstace = "STRT";
        public static string RestartCloudInstace = "RSTR";
        public static string TerminateCloudInstace = "KILL";
        public static string ConfigureDNSRecord = "CDNS";
        public static string RebootCloudInstance = "BOOT";

        public static string ConfigureUsers = "USER";
        public static string UpdateDeployer = "UDEP";
        public static string UpdateLauncher = "ULAU";
        public static string UpdateSiteMaster = "USIT";
        public static string UpdateClient = "UCLT";

        public static string EnableMaintenance = "MNT+";
        public static string DisableMaintenance = "MNT-";

        public static string BackupLauncher = "BLAU";
        public static string UploadLauncherBackup = "LLAU";

        public static string BackupSiteMaster = "BSIT";
        public static string UploadSiteMasterBackup = "LSIT";
        public static string RunSiteMasterCheck = "RSMC";

        public static string RestoreLauncher = "RLAU";

        public static string RestoreSiteMaster = "RSIT";

        public static string PopulateSitemaster = "PSIT";
        public static string PopulateLauncher = "PLAU";
    }
}