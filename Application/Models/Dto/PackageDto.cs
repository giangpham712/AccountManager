namespace AccountManager.Application.Models.Dto
{
    public class PackageDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? FileId { get; set; }
        public FileDto File { get; set; }
        public string Version { get; set; }
    }
}