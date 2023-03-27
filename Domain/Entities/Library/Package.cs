namespace AccountManager.Domain.Entities.Library
{
    public class Package
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? FileId { get; set; }
        public File File { get; set; }
        public string Version { get; set; }
    }
}