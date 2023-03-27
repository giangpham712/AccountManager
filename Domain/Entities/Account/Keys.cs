namespace AccountManager.Domain.Entities.Account
{
    public class Keys : IEntity
    {
        public string ApiPassword { get; set; }
        public string UserTokenPublic { get; set; }
        public string UserTokenPrivate { get; set; }
        public string InterServerPublic { get; set; }
        public string InterServerPrivate { get; set; }
        public string LicensePublic { get; set; }
        public string LicensePrivate { get; set; }
        public string AccountFile { get; set; }
        public byte[] SslPackage { get; set; }
        public string SqlExportPass { get; set; }
        public string LauncherUsers { get; set; }

        public Account Account { get; set; }
        public long? AccountId { get; set; }

        public long Id { get; set; }
    }

    public class LauncherUser
    {
        public string Un { get; set; }
        public string Uc { get; set; }
        public string Fn { get; set; }
        public string Ln { get; set; }
        public string Pw { get; set; }
    }
}